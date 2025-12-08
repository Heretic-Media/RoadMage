using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // Script for handling simple health data
    // Use the correct __Hurtbox collision layer to avoid unintended interactions

    public int maxHealth;
    public int health;

    // Takes no arguments
    [SerializeField] UnityEvent deathEvent;

    private void Start()
    {
        health = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        Damage damageObject = other.gameObject.GetComponent<Damage>();

        if (damageObject != null)
        {
            TakeDamage(damageObject.damage);

            //            Debug.Log(this.gameObject.name + ": " + health);
        }
    }

    // Deal negative damage to heal
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            if (deathEvent.GetPersistentEventCount() == 0)
            {
                Die();
            }
            else
            {
                deathEvent.Invoke();
            }
        }
        else if (health > maxHealth)
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
