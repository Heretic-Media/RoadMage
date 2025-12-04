using UnityEngine;

public class Damage : MonoBehaviour
{
    // script for storing simple attack data

    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        //Health healthObject = collision.gameObject.GetComponent<Health>();

        //if (healthObject != null)
        //{
        //    healthObject.health -= damage;
        //}

        // for now, this projectile instakills enemies
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBehaviour>().Vanish();
        }
        else
        {
            print(collision.gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBehaviour>().Vanish();
        }
        else
        {
            print(collision.gameObject.name);
        }
    }
}
