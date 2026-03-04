using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using XInputDotNetPure; // Required in C#

public class VibrationController : MonoBehaviour
{
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
    [SerializeField] GameObject player;
    private bool inputUsingVibration = false;
    private bool collisionUsingVibration = false;
    private int delayCounter = 0;
    private bool hapticEnabled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hapticEnabled = false; // Default to haptics enabled
    }

    public void ToggleHaptics()
    {
        hapticEnabled = !hapticEnabled; // Toggle the haptic setting
        if (!hapticEnabled)
        {
            GamePad.SetVibration(playerIndex, 0f, 0f); // Stop any ongoing vibration if haptics are disabled
            inputUsingVibration = false;
            collisionUsingVibration = false;
        }
        if (hapticEnabled)
        {
            GamePad.SetVibration(playerIndex, 0.5f, 0.5f);
            collisionUsingVibration = true;
            delayCounter = 50; // Set the delay counter to a certain number of frames (e.g., 20 frames)
        }
        
    }


    private void StopAllRumble()
    {
        GamePad.SetVibration(playerIndex, 0f, 0f);
        if (inputUsingVibration)
        {
            inputUsingVibration = false;
        }
        if (collisionUsingVibration)
        {
            collisionUsingVibration = false;
        }
    }

    private void isAbilityActive()
    {
        var gp = Gamepad.current;

        if (gp != null)
        {
            if (gp.aButton.isPressed && Time.timeScale != 0)
            {
                GamePad.SetVibration(playerIndex, 0.05f, 0.05f);
                inputUsingVibration = true;
            }

            else
            {
                inputUsingVibration = false;
                if (!collisionUsingVibration)
                {
                    GamePad.SetVibration(playerIndex, 0f, 0f);
                }
            }

            if (player.transform.childCount > 0)
            {
                for (int i = 0; i < player.transform.childCount; i++)
                {
                    if (player.transform.GetChild(i).name == "ForwardAbility(Clone)" && gp.leftShoulder.isPressed)
                    {
                        GamePad.SetVibration(playerIndex, 0.2f, 0.2f);
                        inputUsingVibration = true;
                    }

                    //if (player.transform.GetChild(i).name == "[name here](Clone)" && gp.[button].isPressed)
                    //{
                    //GamePad.SetVibration(playerIndex, 0.3f, 0.3f);
                    //return;
                    //}


                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !inputUsingVibration && hapticEnabled)
        {
            if (Gamepad.current != null)
            {
                GamePad.SetVibration(playerIndex, 0.2f, 0.2f);
            }
            collisionUsingVibration = true;
            delayCounter = 20; // Set the delay counter to a certain number of frames (e.g., 20 frames)
        }
        if (collision.gameObject.CompareTag("Building") && !inputUsingVibration && hapticEnabled)
        {
            if (Gamepad.current != null)
            {
                GamePad.SetVibration(playerIndex, 0.5f, 0.5f);
            }
            collisionUsingVibration = true;
            delayCounter = 50; // Set the delay counter to a certain number of frames (e.g., 20 frames)
        }
    }


    public void bigRumble()
    {         
        GamePad.SetVibration(playerIndex, 1f, 1f);
        collisionUsingVibration = true;
        delayCounter = 20; // Set the delay counter to a certain number of frames (e.g., 20 frames)
    }

    // Update is called once per frame
    void Update()
    {
        if (Gamepad.current != null)
        {
            if (hapticEnabled)
            {
                isAbilityActive();

                if (!collisionUsingVibration)
                {
                    if (!inputUsingVibration)
                    {
                        GamePad.SetVibration(playerIndex, 0f, 0f);
                    }
                }

                if (delayCounter == 0)
                {
                    collisionUsingVibration = false;
                }
                else if (delayCounter > 0)
                {
                    delayCounter--;
                }
            }
        }
        else
        {
            StopAllRumble();
            hapticEnabled = false; // Disable haptics if no gamepad is connected
        }
    }
  
}
