using UnityEngine;

public class FollowRigidbody : MonoBehaviour
{
    public GameObject target;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = target.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = rb.position;
    }
}
