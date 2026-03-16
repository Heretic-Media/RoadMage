using UnityEngine;
using UnityEngine.InputSystem;

public class ForwardAbility : MonoBehaviour
{
    Rigidbody playerRigidbody;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject audioManager;
    [SerializeField] float speedThreshold = 5;
    [SerializeField] float damageMult = 1f;
    public int element = 0;
    public int attackCooldown = 60;
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
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("<sprite=123> to fire a blast while moving fast.", 6f);
        }
        else if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().KeyboardConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Hold <sprite=120> to fire a blast while moving fast.", 6f);
        }
        
    }

    private void FixedUpdate()
    {
        float forwardVel = transform.InverseTransformDirection(playerRigidbody.linearVelocity).z;

        if (forwardVel >= speedThreshold && attackCooldown <= 0 && !indicatorParticles.isPlaying)
        {
            indicatorParticles.Play();
        }
        else if (forwardVel < speedThreshold || attackCooldown > 0)
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
            FireProjectile(damageMult * forwardVel, 1.5f * playerRigidbody.linearVelocity);
            attackCooldown = 60;
        }
        else if (attackCooldown > 0)
        {
            attackCooldown--;
        }
    }

    private void StartAudio()
    {
        audioManager.SetActive(true);
        Invoke("CutAudio", 1.5f);
    }

    private void CutAudio()
    {
        audioManager.SetActive(false);
    }

    void FireProjectile(float damage, Vector3 velocity)
    {
        Camera.main.GetComponent<CameraBehaviour>().Shake(0.7f, 0.2f);

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
        StartAudio();
        GameObject newProj = Instantiate(projectile, transform.position, Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f));
        newProj.SetActive(true);
        newProj.transform.GetComponentInChildren<Damage>().damage = (int)damage;
        velocity.y = 0f;
        newProj.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.Impulse);
    }
}
