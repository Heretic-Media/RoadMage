using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBookBehaviour : MonoBehaviour
{
    public enum State { Patrolling, Chasing, Attacking }
    private State currentState = State.Patrolling;

    [Tooltip("Speed at which the enemy travels.")]
    [SerializeField] private float movementSpeed = 2f;

    [Tooltip("The range at which the enemy notices and chases the player.")]
    [SerializeField] private float visionDistance = 30f;

    [Tooltip("The distance in units this enemy will shoot the player from.")]
    [SerializeField] private float attackRange = 1.5f;

    [Tooltip("Time in seconds between melee attacks.")]
    [SerializeField] private float attackCooldown = 0.5f;

    [Tooltip("Damage dealt by attacks.")]
    [SerializeField] private int damage = 1;

    [Tooltip("Prefab spawned when this enemy dies.")]
    [SerializeField] private GameObject deathCry;

    [Tooltip("Camera shake duration when this enemy dies.")]
    [SerializeField] private float cameraShakeDuration = 0.1f;

    [Tooltip("Camera shake magnitude when this enemy dies.")]
    [SerializeField] private float cameraShakeMagnitude = 0.05f;

    [SerializeField] private int PointValue = 1;

    // Patrol area bounds
    [SerializeField] protected Vector3 patrolAreaMin = new Vector3(-20, 0, -20);
    [SerializeField] protected Vector3 patrolAreaMax = new Vector3(20, 0, 20);

    private GameObject playerObject;
    private Rigidbody rb;
    private float attackTimer = 0f;

    // Random patrol
    private Vector3 randomPatrolTarget;
    private float patrolTargetTimeout = 0f;
    private const float patrolTargetInterval = 4f;

    [Tooltip("If true, the enemy will not despawn.")]
    [SerializeField] private bool persistent = true;

    private Animator animator;

    void Start()
    {
        FindPlayer();
        PickNewPatrolTarget();
        // Prevents enemies from pilgrimaging to 0,0
        patrolAreaMin += transform.position;
        patrolAreaMax += transform.position;

        animator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        // Stop if rescue is active
        if (PlayerRescue.Instance != null && PlayerRescue.Instance.IsRescuing())
        {
            if (rb != null)
                rb.linearVelocity = Vector3.zero;
            return;
        }

        if (playerObject == null)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0)
            {
                Debug.LogWarning("Follow_player: player Transform is not assigned.");
            }
            else
            {
                playerObject = players[0];
            }
        }
        else if ((transform.position - playerObject.transform.position).sqrMagnitude < 60 * 60)
        {
            switch (currentState)
            {
                case State.Patrolling:
                    Patrol();
                    if (VisionCheck())
                        currentState = State.Chasing;
                    break;

                case State.Chasing:
                    Chase();
                    if (!VisionCheck())
                        currentState = State.Patrolling;
                    else if (RangeCheck())
                        currentState = State.Attacking;
                    break;

                case State.Attacking:
                    rb.linearVelocity = Vector3.zero;
                    attackTimer += Time.fixedDeltaTime;
                    if (attackTimer > attackCooldown)
                    {
                        attackTimer -= attackCooldown;
                        AttackPlayer();
                    }
                    if (!RangeCheck())
                        currentState = State.Chasing;
                    break;
            }
        }
        else if (!persistent)
        {
            // despawn if we're not close enough to the player
            Destroy(gameObject);
        }
    }

    public void Vanish()
    {

        if (deathCry != null)
        {
            Instantiate(deathCry, transform.position, transform.rotation);
        }

        // shake the camera
        if (GameObject.FindGameObjectWithTag("MainCamera") != null)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraBehaviour>().Shake(cameraShakeDuration, cameraShakeMagnitude);
        }
        else
        {
            print("can't find camera");
        }

        GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>().AddScore(PointValue);

        Destroy(gameObject);
    }

    private bool VisionCheck()
    {
        Vector3 diff = playerObject.transform.position - transform.position;
        float distSqrd = diff.sqrMagnitude;
        return distSqrd < visionDistance * visionDistance;
    }

    private bool RangeCheck()
    {
        Rigidbody playerRigidbody = playerObject.GetComponent<Rigidbody>();
        Vector3 diff = playerObject.transform.position - transform.position;
        float distSqrd = diff.sqrMagnitude;
        return distSqrd < attackRange * attackRange;
    }


    void AttackPlayer()
    {
        if (RangeCheck())
        {
            animator.SetBool("Book Attacking", true);
        }
    }

    void Patrol()
    {
        patrolTargetTimeout -= Time.fixedDeltaTime;
        Vector3 direction = (randomPatrolTarget - transform.position);
        direction.y = 0; // keep movement horizontal
        float distance = direction.magnitude;

        if (distance < 0.5f || patrolTargetTimeout <= 0f)
        {
            PickNewPatrolTarget();
            direction = (randomPatrolTarget - transform.position);
            direction.y = 0;
        }

        rb.linearVelocity = direction.normalized * movementSpeed;
    }

    protected void PickNewPatrolTarget()
    {
        float x = Random.Range(patrolAreaMin.x, patrolAreaMax.x);
        float z = Random.Range(patrolAreaMin.z, patrolAreaMax.z);
        randomPatrolTarget = new Vector3(x, transform.position.y, z);
        patrolTargetTimeout = patrolTargetInterval;
    }

    void Chase()
    {
        // stripped down from ChaseWithFormation for this enemy type

        Vector3 targetPosition = playerObject.transform.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = direction * movementSpeed;
    }

    protected void FindPlayer()
    {
        if (playerObject == null)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0)
            {
                Debug.LogWarning("Follow_player: player Transform is not assigned.");
            }
            else
            {
                playerObject = players[0];
            }
        }
        rb = GetComponent<Rigidbody>();
    }
}