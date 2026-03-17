using UnityEngine;

public class FieryDriftProjectile : MonoBehaviour
{
    // despawnOnHit triggers on environment collision, NOT enemy collision
    public bool despawnOnHit = true;
    public bool despawnAfterTime = true;
    public int despawnTimer = 6;

    public float timeAlive = 0;

    [SerializeField] private GameObject particleSystemHolder;
    [SerializeField] private GameObject hitBoxHolder;

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
            particleSystemHolder.transform.position = hit.point;
            hitBoxHolder.transform.position = hit.point;
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