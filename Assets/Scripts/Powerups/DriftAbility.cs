using UnityEngine;

public class DriftAbility : MonoBehaviour
{
    [SerializeField] private GameObject projectile;

    [Tooltip("Toggle the ability on or off")]
    public bool enableDriftProjectiles = true;

    [Tooltip("Time spent drifting for debugging")]
    [SerializeField] private float driftTime = 0;
    private float driftDelayTime = 0;

    [Tooltip("Drift time needed before spawning first projectile")]
    [SerializeField] private float driftAbilityDelay = 1f;
    [Tooltip("Drift time needed before projectile speed starts decaying")]
    [SerializeField] private float driftAbilityDecay = 4f;
    [Tooltip("Drift time needed before projectile stop spawning")]
    [SerializeField] private float driftAbilityBurnout = 10f;
    [Tooltip("The fire rate of the projectiles")]
    [SerializeField] private float driftProjectileRate = 0.05f;
    [Tooltip("The random spawn direction range")]
    [SerializeField] private float driftProjectileRandomness = 0.3f;

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
            if (driftDelayTime < driftAbilityDelay) 
            {
                driftDelayTime += Time.deltaTime;
            }
            else 
            {
                if (driftTime >= driftAbilityBurnout)
                {
                    driftTime = driftAbilityBurnout;
                }
                else
                {
                    driftTime += Time.deltaTime;
                }
            }
        }
        else 
        {
            driftDelayTime = 0;

            if (driftTime <= 0) 
            {
                driftTime = 0;
            }
            else 
            {
                driftTime -= Time.deltaTime;
            } 
        }

        /// Drift Projectiles

        if (carController.drifting && Mathf.Abs(carController.rawSteerInput) > 0.5f && enableDriftProjectiles && driftDelayTime > driftAbilityDelay)
        {
            timeSinceLastDriftProjectile += Time.deltaTime;

            // Debug.Log("spawning drift projectile");
            if (driftTime >= driftAbilityBurnout)
            {
                // Debug.Log("drift ability timed out");
            }
            else if (driftTime <= driftAbilityDecay)
            {
                if (timeSinceLastDriftProjectile >= driftProjectileRate)
                {
                    SpawnProjectile(carController.rb.linearVelocity.magnitude * 0.5f);
                }
            }
            else
            {
                float burnoutTime = (driftAbilityBurnout - driftAbilityDecay);
                float burnoutDifference = burnoutTime - (driftTime - driftAbilityDecay);

                if (timeSinceLastDriftProjectile <= driftProjectileRate * 2 + (1 - burnoutDifference / burnoutTime)) { }
                else if (burnoutDifference > 0)
                {
                    SpawnProjectile(burnoutDifference / burnoutTime
                        * carController.rb.linearVelocity.magnitude * 0.5f);
                }
            }
        }
    }

    private void SpawnProjectile(float projectileSpeed)
    {
        timeSinceLastDriftProjectile = 0;

        if (projectile == null)
            return;

        Vector3 spawnPos = transform.position - transform.forward * 0.6f + Vector3.up * 0.2f;
        GameObject proj = Instantiate(projectile, spawnPos, Quaternion.identity);
        proj.SetActive(true);

        float rnd = Random.Range(0f, driftProjectileRandomness);

        Vector3 dir = -transform.forward * (1 - ((driftProjectileRandomness / 2f) - rnd));
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
