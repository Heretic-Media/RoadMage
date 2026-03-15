using UnityEngine;
using UnityEngine.InputSystem;

public class HealAbility : MonoBehaviour
{
    private bool buttonPressed;
    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject healArea;
    [SerializeField] private GameObject audio;
    [SerializeField] private float cooldown = 10f;
    //[SerializeField] private int healAmount = 75;
    [SerializeField] private int healAmount = 75;
    private GameObject[] debuffs;
    public bool healOnCooldown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

        debuffs = GameObject.FindGameObjectsWithTag("DeBuff");
    }

    private void StartParticleEffect()
    {
        particles.SetActive(true);
        audio.SetActive(true);

        Invoke("StopParticleEffect", 2f);
    }

    private void StopParticleEffect()
    {
        particles.SetActive(false);
        audio.SetActive(false);
    }

    private void Cleanse()
    {
        foreach (var debuff in debuffs)
        {
            if (debuff.name == "stun")
            {
                debuff.GetComponent<StunDebuff>().UnStun();
                debuff.SetActive(false);
            }
        }
    }

    private void Heal()
    {
        if (!healOnCooldown)
        {
            if (debuffs.Length > 0)
            {
                Cleanse();
            }
            GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().TakeDamage(-healAmount);

            Instantiate(healArea, transform.position, transform.rotation).SetActive(true);
            StartParticleEffect();
            healOnCooldown = true;
        }
        else
        {
            Invoke("RemoveCooldown", cooldown);
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
        debuffs = GameObject.FindGameObjectsWithTag("DeBuff");

        var kb = Keyboard.current;
        var gp = Gamepad.current;

            if (kb != null && kb.eKey.isPressed || gp != null && gp.xButton.isPressed)
            {
                    Heal();
                    return;
            }

    }
}
