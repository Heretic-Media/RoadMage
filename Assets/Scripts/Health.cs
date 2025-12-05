using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // Script for handling simple health data
    // Use the correct __Hurtbox collision layer to avoid unintended interactions

    public int maxHealth;
    public int health;

    // Takes no arguments
    public UnityEvent deathEvent;

    private void Start()
    {
        health = maxHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Damage damageObject = collision.gameObject.GetComponent<Damage>();

        if (damageObject != null)
        {
            health -= damageObject.damage;
            if (health <= 0)
            {
                if (deathEvent == null)
                {
                    Die();
                }
                else
                {
                    deathEvent.Invoke();
                }
            }

//            Debug.Log(this.gameObject.name + ": " + health);
        }
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    // Death defaults to this function if deathEvent is left blank
    public virtual void Die()
    {
        Destroy(this.gameObject);
    }
}
