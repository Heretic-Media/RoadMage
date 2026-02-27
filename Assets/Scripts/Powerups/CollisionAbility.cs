using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CollisionAbility : MonoBehaviour
{

    [Tooltip("Toggle the ability on or off")]
    public bool enableCollisionAbility = true;

    [SerializeField] private float maxDamage = 5f;

    [SerializeField] private float maxDamageSpeed = 5f;
    [SerializeField] private float minDamageSpeed = 1f;

    public List<UnityEvent> collisionEvents = new List<UnityEvent> { };

    private bool colliding = false;
    string colliderName;

    private Rigidbody rb;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            colliderName = collision.gameObject.name;

            if (!colliding) 
            {
                colliding = true;

                foreach (var collisionEvent in collisionEvents)
                {
                    collisionEvent.Invoke();
                }

                Health enemyDetails = collision.gameObject.GetComponent<Health>();
                if (enemyDetails != null)
                {
                    /// This could instead deal damage to the enemy by calling a public function or changing a public variable
                    /// Then the enemy could be set to despawn once health is 0;

                    if (rb.linearVelocity.magnitude < minDamageSpeed)
                    {
                    }
                    else if (rb.linearVelocity.magnitude >= maxDamageSpeed)
                    {
                        bool enemyKilled = enemyDetails.TakeDamage((int)maxDamage);

                        if (enemyKilled) 
                        {
                            colliding = false;
                        }
                    }
                    else
                    {
                        float damage = ((rb.linearVelocity.magnitude / maxDamageSpeed) * maxDamage);

                        bool enemyKilled = enemyDetails.TakeDamage((int)damage);

                        if (enemyKilled)
                        {
                            colliding = false;
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (colliderName == collision.gameObject.name) 
            {
                colliding = false;
            }
        }
    }
}
