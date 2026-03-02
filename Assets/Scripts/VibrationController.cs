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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void isAbilityActive()
    {

        var kb = Keyboard.current;
        var gp = Gamepad.current;

        if (kb != null || gp != null)
        {
            if (gp.aButton.isPressed)
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
                    Debug.Log("Vibration stopped");
                }
            }

            if (player.transform.childCount > 0)
            {
                for (int i = 0; i < player.transform.childCount; i++)
                {
                    if (player.transform.GetChild(i).name == "ForwardAbility(Clone)" && gp.leftShoulder.isPressed)
                    {
                        GamePad.SetVibration(playerIndex, 0.5f, 0.5f);
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
        if (collision.gameObject.CompareTag("Enemy") && !inputUsingVibration)
        {
            GamePad.SetVibration(playerIndex, 0.2f, 0.2f);
            collisionUsingVibration = true;
            delayCounter = 20; // Set the delay counter to a certain number of frames (e.g., 20 frames)
        }
        if (collision.gameObject.CompareTag("Building") && !inputUsingVibration)
        {
            GamePad.SetVibration(playerIndex, 0.5f, 0.5f);
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
