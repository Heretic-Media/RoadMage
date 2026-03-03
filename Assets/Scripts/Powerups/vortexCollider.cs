using UnityEngine;

public class vortexCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("called vortex on trigger enter");

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Health>().TakeDamage(5);
            Debug.Log("Hit Enemy with Vortex");
        }
        else
        {
            Debug.Log("Vortex hit " + other.gameObject.name);
        }
    }
}
