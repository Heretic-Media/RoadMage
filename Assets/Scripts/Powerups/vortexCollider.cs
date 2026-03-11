using UnityEngine;

public class vortexCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("called vortex on trigger enter");
        // please i made the damage script for a reason
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Health>().TakeDamage(1);
           // Debug.Log("Hit Enemy with Vortex");
        }
        //else
        //{
            //Debug.Log("Vortex hit " + other.gameObject.name);
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Health>().TakeDamage(1);
            //Debug.Log("Hit Enemy with Vortex");
        }
    }
}
