using UnityEngine;
using UnityEngine.InputSystem;

public class UIChangeInputIcons : MonoBehaviour
{
    public GameObject[] keyboardIcons;
    public GameObject[] gamepadIcons;
    public GameObject[] playstationIcons;
    private bool gamepadEnabled = true;
    private bool playstationEnabled = false;
    private bool keyboardEnabled = false;


    private void Start()
    {
        InputSystem.onActionChange += switchIconOnChange;
    }

    void switchIconOnChange(object obj, InputActionChange change)
    {
        if (obj != null && obj is InputAction action)
        { 
            if (action.activeControl == null) return; 
            InputDevice lastDevice = action.activeControl.device;

            if (lastDevice is Gamepad)
            {
                foreach (var icon in gamepadIcons)
                {
                    icon.SetActive(true);
                }
                foreach (var icon in keyboardIcons)
                {
                    icon.SetActive(false);
                }
                foreach (var icon in playstationIcons)
                {
                    icon.SetActive(false);
                }
                gamepadEnabled = true;
                keyboardEnabled = false;
            }
            else if (lastDevice is Keyboard)
            {
                foreach (var icon in gamepadIcons)
                {
                    icon.SetActive(false);
                }
                foreach (var icon in keyboardIcons)
                {
                    icon.SetActive(true);
                }
                foreach (var icon in playstationIcons)
                {
                    icon.SetActive(false);
                }
                gamepadEnabled = false;
                keyboardEnabled = true;
            }
        }
    }

    public bool ControllerConnected()
    {
        return gamepadEnabled;
    }

    public bool KeyboardConnected()
    { 
        return keyboardEnabled; 
    }


    private void Update()
    {
        InputSystem.onActionChange += switchIconOnChange;
    }
}
