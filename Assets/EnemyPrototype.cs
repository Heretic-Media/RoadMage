
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemyPrototype : MonoBehaviour
{
    [Header("Mode")]
    [Tooltip("If true this component acts as a spawner. If false it behaves as an enemy.")]
    [SerializeField] private bool isSpawner = false;

    [Header("General (Enemy)")]
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 0.5f;

    [Header("Spawn (Spawner)")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnDelay = 0.5f;
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Spawn Timing")]
    [Tooltip("If true, spawn interval is chosen randomly between min and max. If false, fixedInterval is used.")]
    [SerializeField] private bool useRandomInterval = true;
    [SerializeField] private float fixedInterval = 2f;
    [SerializeField] private float randomIntervalMin = 1f;
    [SerializeField] private float randomIntervalMax = 3f;
    [SerializeField] private int maxActiveEnemies = 0;
    [SerializeField] private float spawnRadius = 0f;

    [Header("Ranged Attack (Enemy)")]
    [Tooltip("The enemy will shoot this at the player")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("The speed at which the projectile travels.")]
    [SerializeField] private float projectileSpeed = 10f;
    [Tooltip("The enemy number of times an enemy can fire a shot every second.")]
    [SerializeField] private float fireRate = 1f;
    [Tooltip("If the player is beyond this range, the enemy will not shoot.")]
    [SerializeField] private float shootingRange = 8f;
    [SerializeField] private float projectileLifetime = 5f;

    [Header("Vanish / Close Combat (Enemy)")]
    [SerializeField] private GameObject vanishEffect;
    [SerializeField] private float closeVanishSpeedThreshold = 0.5f; // Lowered from 2f to 0.5f

    [Header("Audio")]
    [SerializeField] private AudioClip onHitCrySFXClip;

    private Rigidbody rb;
    private bool isActive = false;
    private float nextFireTime = 0f;

    private Coroutine spawnCoroutine;
    private int activeCount = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        // Ensure physics collisions and callbacks occur when used as enemy:
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Prevent enemy from falling over
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Optionally, set mass and drag for stability
        rb.mass = 1f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;

        // Ensure collider is a sphere for better stability
        Collider col = GetComponent<Collider>();
        if (col != null && !(col is SphereCollider))
        {
            Destroy(col);
            col = gameObject.AddComponent<SphereCollider>();
        }

        
    }

    private void Start()
    {
        if (isSpawner)
        {
            if (spawnOnStart)
                StartSpawnerWithDelay(spawnDelay);
            return;
        }

        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        if (spawnEffect != null)
            Instantiate(spawnEffect, transform.position, Quaternion.identity);

        StartCoroutine(ActivateAfterDelay(spawnDelay));
    }

    public void StartSpawnerWithDelay(float delay)
    {
        if (!isSpawner) return;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnRoutine(delay));
    }

    public void StopSpawner()
    {
        if (!isSpawner) return;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine(float initialDelay)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, initialDelay));

        while (true)
        {
            if (maxActiveEnemies <= 0 || activeCount < maxActiveEnemies)
            {
                SpawnEnemyInstance();
            }

            float wait = useRandomInterval
                ? Random.Range(Mathf.Max(0f, randomIntervalMin), Mathf.Max(0f, randomIntervalMax))
                : Mathf.Max(0f, fixedInterval);

            yield return new WaitForSeconds(wait);
        }
    }

    private void SpawnEnemyInstance()
    // will check but i think this function shouldnt be here
    {
        if (enemyPrefab == null) return;

        Vector3 spawnPos = transform.position;
        if (spawnRadius > 0f)
            spawnPos += Random.insideUnitSphere * spawnRadius;

        GameObject inst = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        activeCount++;

        var ep = inst.GetComponent<EnemyPrototype>();
        if (ep != null)
        {
            ep.isSpawner = false;
            ep.spawnEffect = spawnEffect;
            ep.projectileLifetime = Mathf.Max(0f, projectileLifetime);
            var tracker = inst.GetComponent<SpawnedEnemyTracker>();
            if (tracker == null) tracker = inst.AddComponent<SpawnedEnemyTracker>();
            tracker.Initialize(this);
        }

        if (spawnEffect != null)
            Instantiate(spawnEffect, spawnPos, Quaternion.identity);
    }

    internal void NotifySpawnedEnemyDestroyed()
    {
        activeCount = Mathf.Max(0, activeCount - 1);
    }

    private IEnumerator ActivateAfterDelay(float delay)
    {
        isActive = false;
        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        isActive = true;
        nextFireTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (isSpawner)
            return;

        if (!isActive || target == null || rb == null)
            return;

        Vector3 toTarget = target.position - rb.position;
        float sqrDist = toTarget.sqrMagnitude;
        float stopSqr = stopDistance * stopDistance;
        float shootSqr = shootingRange * shootingRange;

        if (sqrDist > stopSqr)
        {
            Vector3 next = Vector3.MoveTowards(rb.position, target.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(next);

            Vector3 lookDir = target.position - rb.position;
            if (lookDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                rb.MoveRotation(targetRot);
            }
        }

        if (sqrDist <= shootSqr && sqrDist > stopSqr)
        {
            float clampedFireRate = Mathf.Max(0.0001f, fireRate);
            if (Time.time >= nextFireTime)
            {
                FireProjectile();
                nextFireTime = Time.time + (1f / clampedFireRate);
            }
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || target == null)
            return;

        Vector3 spawnPos = transform.position + transform.forward * 0.6f + Vector3.up * 0.2f;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Vector3 dir = (target.position - spawnPos).normalized;
        Rigidbody projRb = proj.GetComponent<Rigidbody>();
        if (projRb == null)
        {
            projRb = proj.AddComponent<Rigidbody>();
            projRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRb.interpolation = RigidbodyInterpolation.Interpolate;
            projRb.useGravity = false;
        }

        projRb.linearVelocity = dir * projectileSpeed;

        if (projectileLifetime > 0f)
            Destroy(proj, projectileLifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isSpawner)
            return;

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

    private void OnTriggerEnter(Collider other)
    {
        if (isSpawner)
            return;

        if (!other.gameObject.CompareTag("Player"))
            return;

        float playerSpeed = 0f;
        if (other.attachedRigidbody != null)
            playerSpeed = other.attachedRigidbody.linearVelocity.magnitude;
        else
        {
            var prb = other.gameObject.GetComponent<Rigidbody>();
            if (prb != null)
                playerSpeed = prb.linearVelocity.magnitude;
        }

        if (playerSpeed >= closeVanishSpeedThreshold)
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.AddXP(10);
            Vanish();
        }
    }

    private void Vanish()
    {
        if (vanishEffect != null)
            Instantiate(vanishEffect, transform.position, Quaternion.identity);

        AudioSource.PlayClipAtPoint(onHitCrySFXClip, new Vector3(transform.position.x, 35, transform.position.z));
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        if (speed < 0f) speed = 0f;
        if (stopDistance < 0f) stopDistance = 0f;
        if (shootingRange < 0f) shootingRange = 0f;
        if (fireRate < 0f) fireRate = 0f;
        if (projectileSpeed < 0f) projectileSpeed = 0f;
        if (spawnDelay < 0f) spawnDelay = 0f;
        if (closeVanishSpeedThreshold < 0f) closeVanishSpeedThreshold = 0f;
        if (fixedInterval < 0f) fixedInterval = 0f;
        if (randomIntervalMin < 0f) randomIntervalMin = 0f;
        if (randomIntervalMax < 0f) randomIntervalMax = 0f;
        if (projectileLifetime < 0f) projectileLifetime = 0f;
        if (spawnRadius < 0f) spawnRadius = 0f;
        if (maxActiveEnemies < 0) maxActiveEnemies = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (!isSpawner)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stopDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, shootingRange);
        }
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}

/// <summary>
/// Helper attached to spawned enemy instances so the spawner knows when they are destroyed.
/// </summary>
public class SpawnedEnemyTracker : MonoBehaviour
{
    private EnemyPrototype spawner;

    public void Initialize(EnemyPrototype owner)
    {
        spawner = owner;
    }

    private void OnDestroy()
    {
        if (spawner != null)
            spawner.NotifySpawnedEnemyDestroyed();
    }
}
