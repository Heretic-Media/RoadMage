using UnityEngine;

public class GarageEnemyBehaviour : EnemyBehaviour
{
    private void Start()
    {
        // Patrol area bounds
        patrolAreaMin += transform.position;
        patrolAreaMax += transform.position;

        FindPlayer();
        PickNewPatrolTarget();


    }
}
