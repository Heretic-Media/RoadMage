using UnityEngine;
using UnityEngine.InputSystem;

public class ForwardAbility : MonoBehaviour
{
    Rigidbody playerRigidbody;
    [SerializeField] private GameObject projectile;
    [SerializeField] float speedThreshold = 5;
    private int attackCooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponentInParent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float forwardVel = transform.InverseTransformDirection(playerRigidbody.linearVelocity).z;

        var kb = Keyboard.current;
        var gp = Gamepad.current;

        bool handbrake =
            (gp != null && gp.leftShoulder.isPressed) ||
            (kb != null && kb[Key.LeftCtrl].isPressed);

        if (handbrake && (forwardVel >= speedThreshold) && attackCooldown <= 0)
        {
            FireProjectile(2 * (int)forwardVel, 1.5f * playerRigidbody.linearVelocity);
            attackCooldown = 60;
        }
        else if (attackCooldown > 0)
        {
            attackCooldown--;
        }
    }

    void FireProjectile(int damage, Vector3 velocity)
    {
        GameObject newProj = Instantiate(projectile, transform.position, transform.rotation);
        newProj.SetActive(true);
        newProj.transform.GetComponentInChildren<Damage>().damage = damage;
        newProj.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.Impulse);
    }
}
