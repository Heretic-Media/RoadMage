using UnityEngine;
using UnityEngine.InputSystem;

public class ForwardAbility : MonoBehaviour
{
    Rigidbody playerRigidbody;
    [SerializeField] private GameObject projectile;
    [SerializeField] float speedThreshold = 5;
    public int element = 0;
    private int attackCooldown = 60;
    [SerializeField] ParticleSystem indicatorParticles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponentInParent<Rigidbody>();
    }

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().ControllerConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("<sprite=123> to fire a blast while moving fast.", 4f);
        }
        else if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().KeyboardConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Hold <sprite=120> to fire a blast while moving fast.", 4f);
        }
        
    }

    private void FixedUpdate()
    {
        float forwardVel = transform.InverseTransformDirection(playerRigidbody.linearVelocity).z;

        if (forwardVel >= speedThreshold && attackCooldown <= 0)
        {
            indicatorParticles.Play();
        }
        else
        {
            indicatorParticles.Stop();
        }

        var kb = Keyboard.current;
        var gp = Gamepad.current;

        bool handbrake =
            (gp != null && gp.leftShoulder.isPressed) ||
            (kb != null && kb[Key.LeftCtrl].isPressed);

        if (handbrake && (forwardVel >= speedThreshold) && attackCooldown <= 0)
        {
            FireProjectile(forwardVel, 1.5f * playerRigidbody.linearVelocity);
            attackCooldown = 60;
        }
        else if (attackCooldown > 0)
        {
            attackCooldown--;
        }
    }

    void FireProjectile(float damage, Vector3 velocity)
    {
        switch (element)
        {
            case 0:
                break;

            case 1:
                damage *= 1.2f;
                break;

            case 2:
                damage *= 1.5f;
                velocity *= 0.5f;
                break;

            case 3:
                velocity *= 0.8f;
                break;

            case 4:
                damage *= 0.5f;
                velocity *= 1.5f;
                break;
        }
        GameObject newProj = Instantiate(projectile, transform.position, transform.rotation);
        newProj.SetActive(true);
        newProj.transform.GetComponentInChildren<Damage>().damage = (int)damage;
        newProj.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.Impulse);
    }
}
