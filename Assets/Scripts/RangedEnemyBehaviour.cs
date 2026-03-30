using UnityEngine;

public class RangedEnemyBehaviour : MonoBehaviour
{
    public enum State { Patrolling, Strafing, Shooting }
    private State currentState = State.Patrolling;

    [Tooltip("Speed at which the enemy travels.")]
    [SerializeField] private float movementSpeed = 2f;

    [Tooltip("The range at which the enemy notices the player.")]
    [SerializeField] private float visionDistance = 200f;

    [Tooltip("The preferred distance from the player when strafing.")]
    [SerializeField] private float strafeDistance = 10f;

    [Tooltip("Time in seconds between shots.")]
    [SerializeField] private float attackCooldown = 1.5f;

    [Tooltip("Prefab spawned as a bullet toward the player.")]
    [SerializeField] private GameObject bulletPrefab;

    [Tooltip("Transform position where bullets spawn.")]
    [SerializeField] private Transform bulletSpawnPoint;

    [Tooltip("Camera shake duration when this enemy dies.")]
    [SerializeField] private float cameraShakeDuration = 0.1f;

    [Tooltip("Camera shake magnitude when this enemy dies.")]
    [SerializeField] private float cameraShakeMagnitude = 0.05f;

    [Tooltip("Prefab spawned when this enemy dies.")]
    [SerializeField] private GameObject deathCry;

    [SerializeField] private int PointValue = 1;

    // Patrol area bounds
    [SerializeField] protected Vector3 patrolAreaMin = new Vector3(-20, 0, -20);
    [SerializeField] protected Vector3 patrolAreaMax = new Vector3(20, 0, 20);

    // Strafe direction (1 = clockwise, -1 = counter-clockwise)
    private float strafeDirection = 1f;
    private float attackTimer = 0f;

    // Random patrol
    private Vector3 randomPatrolTarget;
    private float patrolTargetTimeout = 0f;
    private const float patrolTargetInterval = 4f;

    [Tooltip("If true, the enemy will not despawn.")]
    [SerializeField] private bool persistent = true;

    private GameObject playerObject;
    private Rigidbody rb;
    private CameraBehaviour cachedCamera;
    private ScoreManager cachedScoreManager;
    private float visionDistanceSqr;

    void Start()
    {
        FindPlayer();
        PickNewPatrolTarget();
        patrolAreaMin += transform.position;
        patrolAreaMax += transform.position;

        visionDistanceSqr = visionDistance * visionDistance;

        GameObject camObj = GameObject.FindWithTag("MainCamera");
        if (camObj != null) cachedCamera = camObj.GetComponent<CameraBehaviour>();

        GameObject scoreObj = GameObject.FindWithTag("ScoreManager");
        if (scoreObj != null) cachedScoreManager = scoreObj.GetComponent<ScoreManager>();

        // Randomize initial strafe direction
        strafeDirection = Random.value > 0.5f ? 1f : -1f;
    }

    void FixedUpdate()
    {
        if (PlayerRescue.Instance != null && PlayerRescue.Instance.IsRescuing())
        {
            if (rb != null)
                rb.linearVelocity = Vector3.zero;
            return;
        }

        if (playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");
            if (playerObject == null) return;
        }

        float distSqr = (transform.position - playerObject.transform.position).sqrMagnitude;

        if (distSqr < visionDistanceSqr)
        {
            switch (currentState)
            {
                case State.Patrolling:
                    Patrol();
                    currentState = State.Strafing;
                    break;

                case State.Strafing:
                    Strafe();
                    if (attackTimer <= 0f)
                        currentState = State.Shooting;
                    break;

                case State.Shooting:
                    Shoot();
                    currentState = State.Strafing;
                    break;
            }
        }
        else if (currentState != State.Patrolling)
        {
            currentState = State.Patrolling;
        }
        else if (!persistent)
        {
            Destroy(gameObject);
        }
    }

    void Patrol()
    {
        patrolTargetTimeout -= Time.fixedDeltaTime;
        Vector3 direction = randomPatrolTarget - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.25f || patrolTargetTimeout <= 0f)
        {
            PickNewPatrolTarget();
            direction = randomPatrolTarget - transform.position;
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

    void Strafe()
    {
        Vector3 toPlayer = playerObject.transform.position - transform.position;
        toPlayer.y = 0;
        float distanceToPlayer = toPlayer.magnitude;

        Vector3 playerDir = toPlayer / distanceToPlayer;

        // Face the player
        if (distanceToPlayer > 0.001f)
        {
            transform.forward = playerDir;
        }

        // Calculate strafe movement (perpendicular to player direction)
        Vector3 strafeDir = new Vector3(-playerDir.z, 0, playerDir.x) * strafeDirection;
        Vector3 targetVelocity = strafeDir * movementSpeed;

        // Adjust velocity to maintain preferred distance from player
        if (distanceToPlayer < strafeDistance - 1f)
        {
            targetVelocity -= playerDir * (movementSpeed * 0.5f);
        }
        else if (distanceToPlayer > strafeDistance + 1f)
        {
            targetVelocity += playerDir * (movementSpeed * 0.5f);
        }

        rb.linearVelocity = targetVelocity;

        attackTimer -= Time.fixedDeltaTime;
    }

    void Shoot()
    {
        rb.linearVelocity = Vector3.zero;

        // Face the player
        Vector3 toPlayer = playerObject.transform.position - transform.position;
        if (toPlayer.sqrMagnitude > 0.001f)
        {
            toPlayer.y = 0;
            transform.forward = toPlayer.normalized;
        }

        // Spawn bullet toward player
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }

        attackTimer = attackCooldown;
    }

    public void Vanish()
    {
        if (deathCry != null)
        {
            Instantiate(deathCry, transform.position, transform.rotation);
        }

        if (cachedCamera != null)
        {
            cachedCamera.Shake(cameraShakeDuration, cameraShakeMagnitude);
        }

        if (cachedScoreManager != null)
        {
            cachedScoreManager.AddScore(PointValue);
        }

        Destroy(gameObject);
    }

    protected void FindPlayer()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");
            if (playerObject == null)
            {
                Debug.LogWarning("RangedEnemyBehaviour: No player found.");
            }
        }
        rb = GetComponent<Rigidbody>();
    }
}
