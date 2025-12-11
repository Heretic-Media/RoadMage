using UnityEngine;

public class Health : MonoBehaviour
{
    // script for storing simple health data

    public int maxHealth;
    public int health;

    public void Die()
    {
        Destroy(this.gameObject);
    }
}
