using UnityEngine;

public class SplittingEnemyBehaviour : EnemyBehaviour
{
    [SerializeField] private GameObject normalEnemyPrefab;
    [SerializeField] private float splitSpacing = 1.5f;

    public override void Vanish()
    {
        if (normalEnemyPrefab != null)
        {
            Vector3 right = transform.right * splitSpacing;
            Instantiate(normalEnemyPrefab, transform.position + right, transform.rotation);
            Instantiate(normalEnemyPrefab, transform.position - right, transform.rotation);
        }

        base.Vanish();
    }

    private void Start()
    {
        patrolAreaMin += transform.position;
        patrolAreaMax += transform.position;

        FindPlayer();
        PickNewPatrolTarget();
        InitHitbox();
    }
}
