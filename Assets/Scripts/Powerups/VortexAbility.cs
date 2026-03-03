using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class VortexAbility : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;
    private bool isActive = false;
    private bool onCooldown = false;
    private bool buttonPressed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().ControllerConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Press <sprite=36> to summon a spinning attack.", 4f);
        }
        else if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().KeyboardConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Press <sprite=100> to summon a spinning attack.", 4f);
        }
    }

    private void startAttack()
    {
        if (!isActive)
        {
            if (!onCooldown)
            {
                hitBox.SetActive(true);
                Invoke("endAttack", 2.5f);
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
            onCooldown = true;
            Invoke("endCooldown", 1.5f);
        }
        else
        {
            return;
        }
    }

    private void endCooldown()
    {
        onCooldown = false;
    }

    public bool IsOnCooldown()
    {
        return onCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        if (kb != null || gp != null)
        {
            if (kb.qKey.isPressed || gp.yButton.isPressed)
            {
                if (!buttonPressed)
                {
                    buttonPressed = true;
                    startAttack();
                    return;
                }
                return;
            }

            else
            {
                buttonPressed = false;
            }
        }
    }
}
