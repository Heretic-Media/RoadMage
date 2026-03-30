using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class VortexAbility : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;
    [SerializeField] private GameObject[] hitBoxes;
    private bool isActive = false;
    [SerializeField] private float cooldown = 5f;
    public bool vortexOnCooldown = false;
    private bool buttonPressed = false;

    private UpgradeAbility upgradeAbility;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().ControllerConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Press <sprite=36> to summon a spinning attack.", 6f);
        }
        else if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().KeyboardConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Press <sprite=100> to summon a spinning attack.", 6f);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeAbility = GetComponent<UpgradeAbility>();
        Upgrade();
    }


    // Update is called once per frame
    void Update()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        if (kb != null && kb.qKey.isPressed || gp != null && gp.yButton.isPressed)
        {
            startAttack();
            return;
        }

        if (upgradeAbility != null && upgradeAbility.upgraded == true)
        {
            Upgrade();
            upgradeAbility.upgraded = false;
        }
    }

    private void startAttack()
    {
        if (!isActive)
        {
            if (!vortexOnCooldown)
            {
                hitBox.SetActive(true);
                Invoke("endAttack", 5f);
                isActive = true;
            }
        }
        else
        {
            return;
        }
    }

    private void endAttack()
    {
        if (isActive)
        {
            hitBox.SetActive(false);
            isActive = false;
            vortexOnCooldown = true;
            Invoke("endCooldown", cooldown);
        }
        else
        {
            return;
        }
    }

    private void endCooldown()
    {
        vortexOnCooldown = false;
    }

    public bool IsOnCooldown()
    {
        return vortexOnCooldown;
    }

    void Upgrade() 
    {
        if (upgradeAbility == null)
        { 
        }
        else if (upgradeAbility.level == 0)
        {
            hitBoxes[0].SetActive(true);
        }
        else if (upgradeAbility.level == 1)
        {
            hitBoxes[0].SetActive(true);
            hitBoxes[1].SetActive(true);
        }
        else if (upgradeAbility.level == 2)
        {
            hitBoxes[0].SetActive(true);
            hitBoxes[1].SetActive(true);
            hitBoxes[2].SetActive(true);
            hitBoxes[3].SetActive(true);
        }
    }
}
