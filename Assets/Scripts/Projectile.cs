using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool despawnOnHit = true;
    public bool despawnAfterTime = true;
    public float despawnTimer = 5;

    [Tooltip("Projectile damage should be set on spawn")]
    public float damage = 0;

    public float timeAlive = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;

        if (despawnAfterTime && timeAlive >= despawnTimer)
        {
            print("projectile timed out");
            Destroy(this.gameObject);
        }
    }


    /// Only deal damage and destroy bullet if it hits enemy (trigger or collision)
    //private void OnTriggerEnter(Collider collision)

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            print(collision.gameObject.name);

            EnemyBehaviour enemyDetails = collision.gameObject.GetComponent<EnemyBehaviour>();
            if (enemyDetails != null)
            {
                /// This could instead deal damage to the enemy by calling a public function or changing a public variable
                /// Then the enemy could be set to despawn once health is 0;

                enemyDetails.TakeDamage(damage);
                print("enemy damaged");
            }

            if (despawnOnHit)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
