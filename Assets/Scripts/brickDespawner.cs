using UnityEngine;

public class brickDespawner : MonoBehaviour
{
    private bool setForDestruction = false;
    private bool checkTime = true;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void DestroyBrick()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Player") && !setForDestruction)
        {
            setForDestruction = true;
            Invoke("DestroyBrick", 1f);
        }
    }

    private bool IsFalling()
    {
        return rb != null && rb.linearVelocity.y < -0.1f;
    }

    private void Update()
    {
        if (checkTime)
        {
            if (Time.timeSinceLevelLoad > 1f)
            {
                checkTime = false;
                if (IsFalling())
                {
                    setForDestruction = true;
                    Invoke("DestroyBrick", 0f);
                }
            }
        }

        if (IsFalling() && !checkTime)
        {
            setForDestruction = true;
            Invoke("DestroyBrick", 10f);
        }

    }
}
