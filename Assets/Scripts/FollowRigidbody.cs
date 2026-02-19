using UnityEngine;

public class FollowRigidbody : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private bool followZ = true;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = target.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        ;
        if (followX) newPosition.x = rb.position.x;
        if (followY) newPosition.y = rb.position.y;
        if (followZ) newPosition.z = rb.position.z;

        transform.position = newPosition;
    }
}
