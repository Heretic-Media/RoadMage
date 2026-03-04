using UnityEngine;
using UnityEngine.InputSystem;

public class HealAbility : MonoBehaviour
{
    private bool buttonPressed;
    [SerializeField] private GameObject[] particles;
    [SerializeField] private GameObject audio;
    [SerializeField] private int healAmount = 75;
    public bool healOnCooldown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].SetActive(false);
        }
    }

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().ControllerConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Press <sprite=35> to drop a healing potion.", 6f);
        }
        else if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().KeyboardConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Press <sprite=64> to drop a healing potion.", 6f);
        }
    }

    private void StartParticleEffect()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].SetActive(true);

        }
        audio.SetActive(true);

        Invoke("StopParticleEffect", 2f);
    }

    private void StopParticleEffect()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].SetActive(false);
        }
        audio.SetActive(false);
    }

    private void Heal()
    {
        if (!healOnCooldown)
        {
            
            GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().TakeDamage(-healAmount);
            StartParticleEffect();
            healOnCooldown = true;
        }
        else
        {
            Invoke("RemoveCooldown", 10f);
        }
    }

    private void RemoveCooldown()
    {
        healOnCooldown = false;
    }

    public bool GetHealOnCooldown()
    {
        return healOnCooldown;
    }

    // Update is called once per frame
    void Update()
    {

        var kb = Keyboard.current;
        var gp = Gamepad.current;

            if (kb != null && kb.eKey.isPressed || gp != null && gp.xButton.isPressed)
            {
                    Heal();
                    return;
            }

    }
}
