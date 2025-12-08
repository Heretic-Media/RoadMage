using UnityEngine;

public class GarageEnemyBehaviour : EnemyBehaviour
{
    private void Start()
    {
        // Patrol area bounds
        patrolAreaMin += transform.parent.transform.parent.position;
        patrolAreaMax += transform.parent.transform.parent.position;

        FindPlayer();
        PickNewPatrolTarget();


    }

    protected override bool VisionCheck()
    {
        if (playerObject.transform.position.x > patrolAreaMin.x && playerObject.transform.position.x < patrolAreaMax.x)
        {
            if (playerObject.transform.position.z > patrolAreaMin.z && playerObject.transform.position.z < patrolAreaMax.z)
            {

                Vector3 diff = playerObject.transform.position - transform.position;
                float distSqrd = diff.sqrMagnitude;
                return distSqrd < visionDistance * visionDistance;
            }
        }
        return false;

    }
}
