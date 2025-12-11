using UnityEngine;

public class KineticBlastProjectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Initialize(int damage, Vector3 velocity)
    {
        GetComponent<Damage>().damage = damage;
        GetComponent<Rigidbody>().AddForce(velocity, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
