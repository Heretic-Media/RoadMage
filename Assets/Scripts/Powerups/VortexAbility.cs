using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class VortexAbility : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;
    private bool isActive = false;
    public bool vortexOnCooldown = false;
    private bool buttonPressed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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
            Invoke("endCooldown", 5f);
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
    }
}
