using UnityEngine;

public class Damage : MonoBehaviour
{
    // script for storing simple attack data

    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        Health healthObject = collision.gameObject.GetComponent<Health>();

        if (healthObject != null)
        {
            healthObject.health -= damage;
        }
    }
}
