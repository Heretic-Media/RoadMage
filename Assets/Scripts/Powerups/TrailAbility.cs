using UnityEngine;

public class TrailAbility : MonoBehaviour
{
    Rigidbody playerRigidbody;
    [SerializeField] GameObject projectile;
    float attackCooldown = 60f;
    float attackTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackTimer = attackCooldown;
        playerRigidbody = transform.parent.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float forwardVel = transform.InverseTransformDirection(playerRigidbody.linearVelocity).z;
        attackTimer -= Mathf.Abs(forwardVel);

        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            GameObject newProj = Instantiate(projectile, transform.position, transform.rotation);
            newProj.SetActive(true);
        }
    }
}
