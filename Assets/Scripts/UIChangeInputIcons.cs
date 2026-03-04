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

    // Subscribe when the component is enabled and unsubscribe when disabled.
    // This prevents callbacks from firing against destroyed/destroying objects.
    private void OnEnable()
    {
        InputSystem.onActionChange += switchIconOnChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= switchIconOnChange;
    }

    void switchIconOnChange(object obj, InputActionChange change)
    {
        if (obj == null || !(obj is InputAction action))
            return;

        // activeControl can be null for some event types — bail out early.
        if (action.activeControl == null)
            return;

        InputDevice lastDevice = action.activeControl.device;

        if (lastDevice is Gamepad)
        {
            if (gamepadIcons != null)
            {
                foreach (var icon in gamepadIcons)
                    if (icon != null) icon.SetActive(true);
            }

            if (keyboardIcons != null)
            {
                foreach (var icon in keyboardIcons)
                    if (icon != null) icon.SetActive(false);
            }

            if (playstationIcons != null)
            {
                foreach (var icon in playstationIcons)
                    if (icon != null) icon.SetActive(false);
            }

            gamepadEnabled = true;
            keyboardEnabled = false;
        }
        else if (lastDevice is Keyboard)
        {
            if (gamepadIcons != null)
            {
                foreach (var icon in gamepadIcons)
                    if (icon != null) icon.SetActive(false);
            }

            if (keyboardIcons != null)
            {
                foreach (var icon in keyboardIcons)
                    if (icon != null) icon.SetActive(true);
            }

            if (playstationIcons != null)
            {
                foreach (var icon in playstationIcons)
                    if (icon != null) icon.SetActive(false);
            }

            gamepadEnabled = false;
            keyboardEnabled = true;
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

    // Removed the Update subscription to avoid adding the delegate every frame.
    private void Update()
    {
        // No longer subscribing here.
    }
}
