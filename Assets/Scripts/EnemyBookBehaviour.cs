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
    [SerializeField] private float visionDistance = 200f;

    [Tooltip("The minimum speed required to damage this enemy.")]
    [SerializeField] private float closeVanishSpeedThreshold = 2f;

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

    [Tooltip("Index of this enemy in the formation.")]
    public int formationIndex = 0;

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

    [Tooltip("How far from the player enemies will try to stay when chasing (formation circle radius).")]
    [SerializeField] private float formationRadius = 0.5f;

    [Tooltip("If true, the enemy will not despawn.")]
    [SerializeField] private bool persistent = true;

    void Start()
    {
        FindPlayer();
        PickNewPatrolTarget();
        // Prevents enemies from pilgrimaging to 0,0
        patrolAreaMin += transform.position;
        patrolAreaMax += transform.position;
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
                    ChaseWithFormation();
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

            PauseManager pause_manager = GameObject.FindGameObjectWithTag("PauseManager").GetComponent<PauseManager>();
            ScoreManager score_manager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();

            pause_manager.InitiateHitStop(0.01f * score_manager.getMultiplier() / 4);
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
        float playerSpeed = playerRigidbody.linearVelocity.magnitude;

        if (playerSpeed >= closeVanishSpeedThreshold)
        {
            return false;
        }
        else
        {
            Vector3 diff = playerObject.transform.position - transform.position;
            float distSqrd = diff.sqrMagnitude;
            return distSqrd < attackRange * attackRange;
        }
    }


    void AttackPlayer()
    {
        if (RangeCheck())
        {
            print("RANGED ATTACK!");
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

    void ChaseWithFormation()
    {
        // Find all active enemies
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> chasingEnemies = new List<GameObject>();

        // Filter only those in Chasing or Attacking state
        foreach (var enemyObj in allEnemies)
        {
            var behaviour = enemyObj.GetComponent<EnemyBookBehaviour>();
            if (behaviour != null && (behaviour.currentState == State.Chasing || behaviour.currentState == State.Attacking))
            {
                chasingEnemies.Add(enemyObj);
            }
        }

        // Sort by distance to player for consistent formation assignment
        chasingEnemies.Sort((a, b) =>
            Vector3.Distance(a.transform.position, playerObject.transform.position)
            .CompareTo(Vector3.Distance(b.transform.position, playerObject.transform.position)));

        int myIndex = chasingEnemies.IndexOf(this.gameObject);
        int totalEnemies = chasingEnemies.Count;
        float radius = formationRadius + Random.Range(-0.3f, 0.3f);

        // Calculate angle for this enemy in the formation circle
        float angle = 0f;
        if (totalEnemies > 0)
        {
            angle = (2 * Mathf.PI / totalEnemies) * myIndex;
        }

        Vector3 formationOffset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        Vector3 targetPosition = playerObject.transform.position + formationOffset;
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