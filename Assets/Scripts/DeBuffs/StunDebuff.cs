using UnityEngine;

public class StunDebuff : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject particles;

    private Rigidbody rb3;

    private void Awake()
    {
        if (target == null) return;
        rb3 = target.GetComponent<Rigidbody>();
    }

    public void Stun(float stunTime)
    {
        if (target == null) return;

        // Stop physics movement
        if (rb3 != null)
        {
            rb3.linearVelocity = Vector3.zero;
            rb3.angularVelocity = Vector3.zero;
            rb3.isKinematic = true;
            rb3.constraints = RigidbodyConstraints.FreezeAll;
        }

        particles.SetActive(true);

        CancelInvoke(nameof(UnStun));
        Invoke(nameof(UnStun), stunTime);
    }

    public void UnStun()
    {
        if (target == null) return;

        if (rb3 != null)
        {
            rb3.isKinematic = false;
            rb3.constraints = RigidbodyConstraints.None;
        }

        particles.SetActive(false);
    }
}
