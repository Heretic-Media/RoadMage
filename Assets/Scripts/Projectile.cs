using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool despawnOnHit = true;
    public bool despawnAfterTime = true;
    public int despawnTimer = 60;

    public int timeAlive = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        timeAlive++;

        if (despawnAfterTime && timeAlive >= despawnTimer)
        {
            //print("projectile timed out");
            Destroy(this.gameObject);
        }
    }

    //TODO: Fix despawnOnHit

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
        if (despawnOnHit)
        {
            Destroy(this.gameObject);
        }
    }

    // Only deal damage and destroy bullet if it hits enemy (trigger or collision)
    /*private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            print(col.gameObject.name);

            EnemyBehaviour enemyDetails = col.gameObject.GetComponent<EnemyBehaviour>();
            if (enemyDetails != null)
            {
                /// This could instead deal damage to the enemy by calling a public function or changing a public variable
                /// Then the enemy could be set to despawn once health is 0;

                enemyDetails.Vanish();
                print("enemy vanished");
            }

            if (despawnOnHit) 
            {
                Destroy(this.gameObject);
            }
        }
    }*/
}
