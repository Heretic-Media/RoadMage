using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    // Only deal damage and destroy bullet if it hits the player (trigger or collision)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerDetails = collision.gameObject.GetComponent<Player>();
            if (playerDetails != null)
            {
                playerDetails.TakeDamage(10);
            }
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerDetails = other.GetComponent<Player>();
            if (playerDetails != null)
            {
                playerDetails.TakeDamage(10);
            }
            Destroy(this.gameObject);
        }
    }
}
