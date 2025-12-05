using UnityEngine;
using UnityEngine.InputSystem;

public class KineticBlast : MonoBehaviour
{
    Rigidbody playerRigidbody;
    [SerializeField] private KineticBlastProjectile projectile;
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

        if (handbrake && (forwardVel >= 15f) && attackCooldown <= 0)
        {
            GameObject newProj = Instantiate(projectile.gameObject, transform.position, transform.rotation);
            newProj.SetActive(true);
            newProj.GetComponent<KineticBlastProjectile>().Initialize(2*(int)forwardVel, 1.5f*playerRigidbody.linearVelocity);
            attackCooldown = 60;
        }
        else if (attackCooldown > 0)
        {
            attackCooldown--;
        }
    }
}
