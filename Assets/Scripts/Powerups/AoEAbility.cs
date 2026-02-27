using UnityEngine;
using UnityEngine.Events;

public class AoEAbility : MonoBehaviour
{
    [SerializeField] private GameObject attack;
    private int attackCooldown = 0;
    CollisionAbility playerCollision;
    UnityEvent attackEvent;
    Rigidbody playerRigidbody;

    float forwardVel = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponentInParent<Rigidbody>();

        playerCollision = FindFirstObjectByType<CollisionAbility>();
        attackEvent = new UnityEvent();
        attackEvent.AddListener(SpawnAttack);
        playerCollision.collisionEvents.Add(attackEvent);
    }

    private void FixedUpdate()
    {
        forwardVel = transform.InverseTransformDirection(playerRigidbody.linearVelocity).z;

        if (attackCooldown > 0)
        {
            attackCooldown--;
        }
    }

    void SpawnAttack()
    {
        if (attackCooldown <= 0)
        {
            GameObject newAttack = Instantiate(attack, transform.position, transform.rotation);
            newAttack.transform.localScale *= Mathf.Clamp(forwardVel / 20, 0.5f, 1.5f);
            newAttack.SetActive(true);
            attackCooldown = 120;
        }
    }
}
