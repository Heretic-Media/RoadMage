using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIPauseMenuFunctions : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject quitConfirmation;
    public GameObject options;
    public GameObject codex;
    private int delayFrames = 0;

    public void Pause()
    {
        Time.timeScale = 0;
        options.SetActive(false);
        codex.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        options.SetActive(false);
        codex.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void OpenOptions()
    {
        pauseMenu.SetActive(false);
        options.SetActive(true);
    }

    public void OpenCodex()
    {
        pauseMenu.SetActive(false);
        codex.SetActive(true);
    }

    public void returnToPauseMenu()
    {
        options.SetActive(false);
        codex.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void AreYouSureYouWantToQuit()
    {         
        pauseMenu.SetActive(false);
        options.SetActive(false);
        codex.SetActive(false);
        quitConfirmation.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        Debug.Log("Returning to main menu...");
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        Debug.Log("Restarting level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
