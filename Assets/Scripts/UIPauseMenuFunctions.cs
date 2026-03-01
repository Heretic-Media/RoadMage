using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIPauseMenuFunctions : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseFirstSelect;
    public GameObject quitConfirmation;
    public GameObject quitFirstSelect;
    public GameObject options;
    public GameObject optionsFirstSelect;
    public GameObject codex;
    public GameObject codexFirstSelect;
    public GameObject eventsSystem;
    private int delayFrames = 0;

    public void Pause()
    {
        Time.timeScale = 0;
        options.SetActive(false);
        codex.SetActive(false);
        pauseMenu.SetActive(true);
        eventsSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(pauseFirstSelect);
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
        eventsSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(optionsFirstSelect);
    }

    public void OpenCodex()
    {
        pauseMenu.SetActive(false);
        codex.SetActive(true);
        eventsSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(codexFirstSelect);
    }

    public void returnToPauseMenu()
    {
        options.SetActive(false);
        codex.SetActive(false);
        quitConfirmation.SetActive(false);
        pauseMenu.SetActive(true);
        eventsSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(pauseFirstSelect);
    }

    public void AreYouSureYouWantToQuit()
    {         
        pauseMenu.SetActive(false);
        options.SetActive(false);
        codex.SetActive(false);
        quitConfirmation.SetActive(true);
        eventsSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(quitFirstSelect);
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
        SceneManager.LoadScene("PortalTransition");
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
