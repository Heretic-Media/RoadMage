using UnityEngine;
using UnityEngine.InputSystem;

public class UIChangeInputIcons : MonoBehaviour
{
    public GameObject[] keyboardIcons;
    public GameObject[] gamepadIcons;
    public GameObject[] playstationIcons;


    void switchIconOnChange(object obj, InputActionChange change)
    {
        if (obj != null && obj is InputAction action)
        { // Modern C# is usable because we're checking the type, not for null
            if (action.activeControl == null) return; // Can't use modern C# here because Destroy exists and does weird things with the memory behind the scenes
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
            }
        }
    }

    private void Update()
    {
        InputSystem.onActionChange += switchIconOnChange;
    }
}
