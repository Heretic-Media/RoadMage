using UnityEngine;
using System.Collections.Generic;

public class EnemyBehaviour : MonoBehaviour
{
    public enum State { Patrolling, Chasing, Attacking }
    private State currentState = State.Patrolling;

    [SerializeField] private float health = 3f;
    [SerializeField] private float maxHealth = 3f;

    [SerializeField] FloatingHealthBar healthBar;

    [Tooltip("Speed at which the enemy travels.")]
    [SerializeField] private float movementSpeed = 2f;

    [Tooltip("The range at which the enemy notices and chases the player.")]
    [SerializeField] private float visionDistance = 200f;

    [Tooltip("The minimum speed required to damage this enemy.")]
    [SerializeField] private float closeVanishSpeedThreshold = 2f;

    [Tooltip("The distance in unit this enemy can damage the player from.")]
    [SerializeField] private float meleeRange = 1f;

    [Tooltip("Time in seconds between melee attacks.")]
    [SerializeField] private float attackCooldown = 0.5f;

    [Tooltip("Prefab spawned when this enemy dies.")]
    [SerializeField] private GameObject deathCry;

    [Tooltip("Camera shake duration when this enemy dies.")]
    [SerializeField] private float cameraShakeDuration = 0.1f;

    [Tooltip("Camera shake magnitude when this enemy dies.")]
    [SerializeField] private float cameraShakeMagnitude = 0.05f;

    [Tooltip("Index of this enemy in the formation.")]
    public int formationIndex = 0;

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

    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }
    void Start()
    {
        FindPlayer();
        PickNewPatrolTarget();
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    void FixedUpdate()
    {
        if (playerObject == null)
        {
            FindPlayer();
            return;
        }

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
                else if (MeleeCheck())
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
                if (!MeleeCheck())
                    currentState = State.Chasing;
                break;
        }
    }

    public void TakeDamage(float damageAmount) 
    {
        health -= damageAmount;
        healthBar.UpdateHealthBar(health, maxHealth);
        if (health <= 0f) 
        {
            Vanish();
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

        Destroy(gameObject);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        float playerSpeed = 0f;
        if (collision.rigidbody != null)
            playerSpeed = collision.rigidbody.linearVelocity.magnitude;
        else
        {
            var prb = collision.gameObject.GetComponent<Rigidbody>();
            if (prb != null)
                playerSpeed = prb.linearVelocity.magnitude;
        }

        if (playerSpeed >= closeVanishSpeedThreshold)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.AddXP(10);
            player.AddScore(5);
            Vanish();
        }
    }

    private bool VisionCheck()
    {
        Vector3 diff = playerObject.transform.position - transform.position;
        float distSqrd = diff.sqrMagnitude;
        return distSqrd < visionDistance * visionDistance;
    }

    private bool MeleeCheck()
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
            return distSqrd < meleeRange * meleeRange;
        }
    }

    void AttackPlayer()
    {
        if (MeleeCheck())
        {
            Player playerDetails = playerObject.GetComponent<Player>();
            if (playerDetails != null)
            {
                playerDetails.TakeDamage(10);
                print("damage dealt");
            }
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
            var behaviour = enemyObj.GetComponent<EnemyBehaviour>();
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
}