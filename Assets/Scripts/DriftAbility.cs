using UnityEngine;

public class DriftAbility : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("Toggle the ability on or off")]
    public bool enableDriftProjectiles = true;

    [SerializeField] private float projectileDamage = 0.5f;

    [Tooltip("Time spent drifting for debugging")]
    [SerializeField] private float driftTime = 0;

    [Tooltip("Drift time needed before spawning first projectile")]
    [SerializeField] private float driftProjectileDelay = 0.5f;
    [Tooltip("Drift time needed before projectile speed scales to player speed")]
    [SerializeField] private float driftProjectileCharge = 1f;
    [Tooltip("The fire rate of the projectiles")]
    [SerializeField] private float driftProjectileRate = 0.05f;

    private float timeSinceLastDriftProjectile = 0;

    private TopDownCarController carController;



    void Start()
    {
        carController = transform.parent.GetComponent<TopDownCarController>();
    }


    void Update()
    {
        transform.position = transform.parent.position;
        
        if (carController.drifting)
        {
            driftTime += Time.deltaTime;
        }
        else { driftTime = 0; }

        /// Drift Projectiles

        if (carController.drifting && Mathf.Abs(carController.rawSteerInput) > 0.5f && enableDriftProjectiles && driftTime > driftProjectileDelay)
        {
            timeSinceLastDriftProjectile += Time.deltaTime;
            if (timeSinceLastDriftProjectile >= driftProjectileRate)
            {
                timeSinceLastDriftProjectile = 0;

                //print("spawning drift projectile");
                if (driftTime - driftProjectileDelay >= driftProjectileCharge)
                {
                    SpawnProjectile(carController.rb.linearVelocity.magnitude * 0.5f);
                }
                else
                {
                    SpawnProjectile((driftTime - driftProjectileDelay) / driftProjectileCharge * carController.rb.linearVelocity.magnitude * 0.5f);
                }
            }
        }
    }

    private void SpawnProjectile(float projectileSpeed)
    {
        if (projectilePrefab == null)
            return;

        projectilePrefab.GetComponent<Projectile>().damage = projectileDamage;

        Vector3 spawnPos = transform.position - transform.forward * 0.6f + Vector3.up * 0.2f;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Vector3 dir = -transform.forward;
        Rigidbody projRb = proj.GetComponent<Rigidbody>();
        if (projRb == null)
        {
            projRb = proj.AddComponent<Rigidbody>();
            projRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRb.interpolation = RigidbodyInterpolation.Interpolate;
            projRb.useGravity = false;
        }
        projRb.linearVelocity = dir * projectileSpeed;
    }
}
