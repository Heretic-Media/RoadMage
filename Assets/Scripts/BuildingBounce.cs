using UnityEngine;

public class BuildingBounce : MonoBehaviour
{
    [SerializeField] private float bounceMultiplier = 3f;
    [SerializeField] private float minBounceForce = 15f;
    [SerializeField] private float maxBounceForce = 20f;
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

                Vector3 force = unitCollisionDirection * collisionRB.linearVelocity.magnitude * (1 + bounceMultiplier);
                Debug.DrawRay(collision.transform.position, force, Color.green, 2f);
                Debug.Log("BOUNCE: " + force.magnitude.ToString());
                if (force.magnitude > maxBounceForce) 
                {
                    collisionRB.AddForce(unitCollisionDirection * maxBounceForce, ForceMode.VelocityChange);
                }
                else if (force.magnitude < minBounceForce) 
                {
                    collisionRB.AddForce(unitCollisionDirection * minBounceForce, ForceMode.VelocityChange);
                }
                else
                {
                    collisionRB.AddForce(force, ForceMode.VelocityChange);
                }
            }
        }
    }
}
