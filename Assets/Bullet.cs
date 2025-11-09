using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
           Player playerDetails = collision.gameObject.GetComponent<Player>();
            playerDetails.TakeDamage(10);
            Destroy(this.gameObject);
        }
    }
}
