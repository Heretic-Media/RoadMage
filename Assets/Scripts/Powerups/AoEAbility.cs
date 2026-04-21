using UnityEngine;
using UnityEngine.Events;

public class AoEAbility : MonoBehaviour
{
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject explosionAudio;
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

    private void Awake()
    {
        GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Hit an enemy to summon a magic attack.", 6f);
    }

    private void FixedUpdate()
    {
        forwardVel = transform.InverseTransformDirection(playerRigidbody.linearVelocity).z;

        if (attackCooldown > 0)
        {
            attackCooldown--;
        }
    }

    private void StartAudio()
    {         
        explosionAudio.SetActive(true);
        Invoke("CutAudio", 1.5f);
    }

    private void CutAudio()
    {
        explosionAudio.SetActive(false);
    }

    void SpawnAttack()
    {
        if (attackCooldown <= 0)
        {
            StartAudio();
            Camera.main.GetComponent<CameraBehaviour>().Shake(0.25f, 1f);
            GameObject newAttack = Instantiate(attack, transform.position, transform.rotation);
            newAttack.transform.localScale *= Mathf.Clamp(forwardVel / 20, 0.5f, 1.5f);
            newAttack.SetActive(true);
            attackCooldown = 120;
        }
    }
}
