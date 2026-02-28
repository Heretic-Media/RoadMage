using UnityEngine;

public class BuildingBounce : MonoBehaviour
{
    [SerializeField] private float bounceMultiplier = 3f;
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("HIT: " + collision.gameObject.name);
        if (collision.gameObject.tag == "Player") 
        {
            //Debug.Log("HIT PLAYER");
            Rigidbody collisionRB = collision.gameObject.GetComponent<Rigidbody>();
            if (collisionRB != null)
            {
                Vector3 collisionDirection = -collision.GetContact(0).normal;
                Vector3 unitCollisionDirection = collisionDirection / collisionDirection.magnitude;

                Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal * 5, Color.red, 2f);

                Vector3 direction = new Vector3(unitCollisionDirection.x, 0, unitCollisionDirection.z);
                float projectedMagnitude = Vector3.Dot(collisionRB.linearVelocity, direction);
                if (projectedMagnitude < 0) projectedMagnitude = 0;

                Debug.Log("BOUNCE: ");
                Debug.DrawRay(collision.transform.position, direction * projectedMagnitude * bounceMultiplier, Color.green, 2f);
                collisionRB.AddForce(direction * projectedMagnitude * bounceMultiplier, ForceMode.Impulse);
            }
        }
    }
}
