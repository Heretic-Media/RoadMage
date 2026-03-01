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
    public GameObject normalOptions;
    public GameObject volumeOptions;
    public GameObject volumeFirstSelect;
    public GameObject codex;
    public GameObject codexFirstSelect;
    public GameObject eventsSystem;
    private bool buttonPressed = false;

    public GameObject deBugMenu;
    private bool deBugOn = false;

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
        deBugMenu.SetActive(false);
        deBugOn = false;
    }

    public void OpenOptions()
    {
        pauseMenu.SetActive(false);
        options.SetActive(true);
        normalOptions.SetActive(true);
        volumeOptions.SetActive(false);
        eventsSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(optionsFirstSelect);
    }

    public void OpenVolumeOptions()
    {
        normalOptions.SetActive(false);
        volumeOptions.SetActive(true);
        eventsSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(volumeFirstSelect);
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

    public void ToggleDebugMenu()
    {
        if (!deBugOn)
        {
            deBugOn = true;
            Time.timeScale = 0;
            options.SetActive(false);
            codex.SetActive(false);
            pauseMenu.SetActive(false);
            deBugMenu.SetActive(true);
        }
        else
        {
            deBugOn = false;
            Time.timeScale = 1;
            options.SetActive(false);
            codex.SetActive(false);
            pauseMenu.SetActive(false);
            deBugMenu.SetActive(false);
        }
    }

    private void Update()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        if (kb != null || gp != null)
        {
            if (kb.escapeKey.isPressed || gp.startButton.isPressed)
            {
                if (!buttonPressed)
                {
                    buttonPressed = true;
                    if (Time.timeScale == 0)
                    {
                        Resume();
                    }
                    else
                    {
                        Pause();
                    }
                    return;
                }
                return;
            }
            if (kb.scrollLockKey.isPressed && kb.insertKey.isPressed)
            {
                ToggleDebugMenu();
                return;
            }

            else
            {
                buttonPressed = false;
                deBugOn = false;
            }
        }
    }
}
