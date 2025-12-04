using UnityEngine;

public class KineticBlastProjectile : MonoBehaviour
{
    [SerializeField] float lifetime = 10f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Initialize(int damage, Vector3 velocity)
    {
        GetComponent<Damage>().damage = damage;
        GetComponent<Rigidbody>().AddForce(velocity * GetComponent<Rigidbody>().mass, ForceMode.Impulse);
        // shake the camera
        if (GameObject.FindGameObjectWithTag("MainCamera") != null)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraBehaviour>().Shake(0.3f, 0.1f);
        }
        else
        {
            print("can't find camera");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
    }

    private void FixedUpdate()
    {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime < 0)
        {
            DestroyImmediate(gameObject);
        }
    }
}
