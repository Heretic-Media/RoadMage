using UnityEngine;
using UnityEngine.InputSystem;

public class UIPauseMenuFunctions : MonoBehaviour
{
    public GameObject pauseMenu;
    private int delayFrames = 0;

    public void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void Quit()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        if (kb != null || gp != null)
        {
            if (kb.escapeKey.isPressed || gp.startButton.isPressed)
            {
                if (delayFrames > 0)
                {
                    --delayFrames;
                    return;
                }
                else if (delayFrames == 0)
                {
                    delayFrames = 50; // Delay for 10 frames to prevent rapid toggling
                    if (Time.timeScale == 0)
                    {
                        Resume();
                    }
                    else
                    {
                        Pause();
                    }

                }
            }
        }
    }
}
