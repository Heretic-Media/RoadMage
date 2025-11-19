using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private AudioClip enemyOnHitSFXClip;

    void Start()
    {

    }

    void Update()
    {

    }

    // Only deal damage and destroy bullet if it hits the player (trigger or collision)
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            print(col.gameObject.name);
            
            Player playerDetails = col.gameObject.GetComponent<Player>();
            if (playerDetails != null)
            {
                AudioSource.PlayClipAtPoint(enemyOnHitSFXClip, new Vector3(transform.position.x, 35, transform.position.z));
                playerDetails.TakeDamage(10);
                print("damage dealt");
            }
            Destroy(this.gameObject);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Player playerDetails = other.GetComponent<Player>();
    //        if (playerDetails != null)
    //        {
    //            playerDetails.TakeDamage(10);
    //        }
    //        Destroy(this.gameObject);
    //    }
    //}
}
