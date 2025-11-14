using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [Tooltip("Speed at which the enemy travels.")]
    [SerializeField] private float movementSpeed = 2f;

    [Tooltip("The range at which the enemy notices and chases the player.")]
    [SerializeField] private float visionDistance = 200f;

    [Tooltip("The minimum speed required to damage this enemy.")]
    [SerializeField] private float closeVanishSpeedThreshold = 2f;

    [Tooltip("The distance in unit this enemy can damage the player from.")]
    [SerializeField] private float meleeRange = 1f;

    [Tooltip("Time in seconds between melee attacks.")]
    [SerializeField] private float attackCooldown = 0.5f;

    private GameObject playerObject;

    private Rigidbody rb;
    private float attackTimer = 0f;

    void FindPlayer()
    {
        // sets up playerObject if we haven't already
        if (playerObject == null)
        {
            // if there are multiple player objects this needs re-writing
            if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
            {
                Debug.LogWarning("Follow_player: player Transform is not assigned.");

            }
            else
            {
                playerObject = GameObject.FindGameObjectsWithTag("Player")[0];

            }

        }

        rb = GetComponent<Rigidbody>();
    }

    private void Vanish()
    {
        // here some logic for death fx can go

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (!collision.gameObject.CompareTag("Player"))
            return;

        float playerSpeed = 0f;
        if (collision.rigidbody != null)
            playerSpeed = collision.rigidbody.linearVelocity.magnitude;
        else
        {
            var prb = collision.gameObject.GetComponent<Rigidbody>();
            if (prb != null)
                playerSpeed = prb.linearVelocity.magnitude;
        }

        if (playerSpeed >= closeVanishSpeedThreshold)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.AddXP(10);
            player.AddScore(5);
            Vanish();
        }
    }

    private bool VisionCheck()
    {
        // returns true if the player is within the vision range of the enemy
        Vector3 diff = playerObject.transform.position - transform.position;
        float distSqrd = diff.sqrMagnitude;
        return distSqrd < visionDistance * visionDistance;
    }

    private bool MeleeCheck()
    {
        // if the player is moving at lethal speed, they can't be attacked
        Rigidbody playerRigidbody = playerObject.GetComponent<Rigidbody>();
        float playerSpeed = playerRigidbody.linearVelocity.magnitude;

        if (playerSpeed >= closeVanishSpeedThreshold)
        {
            return false;
        }
        else
        {
            Vector3 diff = playerObject.transform.position - transform.position;
            float distSqrd = diff.sqrMagnitude;
            return distSqrd < meleeRange * meleeRange;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindPlayer();
    }

    void AttackPlayer()
    {
        if (MeleeCheck())
        {
            Player playerDetails = playerObject.GetComponent<Player>();
            if (playerDetails != null)
            {
                playerDetails.TakeDamage(10);
                print("damage dealt");
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerObject == null)
        {
            FindPlayer();
        }
        else
        {
            if (VisionCheck())
            {
                rb.linearVelocity = (playerObject.transform.position - transform.position).normalized * movementSpeed;
                attackTimer += Time.fixedDeltaTime;

                if (attackTimer > attackCooldown)
                {
                    attackTimer -= attackCooldown;
                    AttackPlayer();
                }

            }
            else
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
    }
}
