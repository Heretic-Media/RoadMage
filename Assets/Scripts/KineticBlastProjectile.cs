using UnityEngine;

public class KineticBlastProjectile : MonoBehaviour
{
    // despawnOnHit triggers on environment collision, NOT enemy collision
    public bool despawnOnHit = true;
    public bool despawnAfterTime = true;
    public int despawnTimer = 6;

    public float timeAlive = 0;

    [SerializeField] private GameObject modelProjectile;
    [SerializeField] private GameObject hitBoxHolder;
    [SerializeField] private GameObject particleSystemHolder;

    LayerMask layerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        transform.position += Vector3.up * 1000;
        layerMask = LayerMask.GetMask("Default");
    }

    private void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;

        if (despawnAfterTime && timeAlive >= despawnTimer)
        {
            //print("projectile timed out");
            Destroy(this.gameObject);
        }

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))

        {
            modelProjectile.transform.position = hit.point + Vector3.up * 0.5f;
            hitBoxHolder.transform.position = hit.point;
            particleSystemHolder.transform.position = hit.point + Vector3.up * 0.5f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collided");
        if (despawnOnHit)
        {
            Destroy(this.gameObject);
        }
    }
}