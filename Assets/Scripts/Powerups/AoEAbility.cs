using UnityEngine;
using UnityEngine.Events;

public class AoEAbility : MonoBehaviour
{
    [SerializeField] private GameObject attack;
    private int attackCooldown = 0;
    CollisionAbility playerCollision;
    UnityEvent attackEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCollision = FindFirstObjectByType<CollisionAbility>();
        attackEvent = new UnityEvent();
        attackEvent.AddListener(SpawnAttack);
        playerCollision.collisionEvents.Add(attackEvent);
    }

    private void Awake()
    {
        GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Hit an enemy to summon a magic attack.", 4f);
    }

    private void FixedUpdate()
    {
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
            newAttack.SetActive(true);
            attackCooldown = 120;
        }
    }
}
