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
    public GameObject tutorialManager;
    private bool tutorialsEnabled = true;
    private bool buttonPressed = false;

    public GameObject deBugMenu;
    private bool deBugOn = false;

    private PauseManager pauseManager;

    public void Pause()
    {
        pauseManager.PauseMenuPause(true);
        options.SetActive(false);
        codex.SetActive(false);
        pauseMenu.SetActive(true);

        eventsSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(pauseFirstSelect);
    }

    public void Resume()
    {
        pauseManager.PauseMenuPause(false);
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

    public void toggleTutorials()
    {
        if (tutorialsEnabled)
        {
            tutorialManager.SetActive(false);
            tutorialsEnabled = false;
            Debug.Log("tutorials off");
            return;
        }

        else if (!tutorialsEnabled)
        {
            tutorialManager.SetActive(true);
            tutorialsEnabled = true;
            Debug.Log("tutorials on");
            return;
        }
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
        SaveSystem.SaveGame();
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

    private void Start()
    {
        pauseManager = GameObject.FindGameObjectWithTag("PauseManager").GetComponent<PauseManager>();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        // Check each device before accessing its keys/buttons
        if ((kb != null && kb.escapeKey.isPressed) || (gp != null && gp.startButton.isPressed))
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
            }
            return;
        }

        // Toggle debug when both scroll lock and insert are pressed on keyboard
        if (kb != null && kb.scrollLockKey.isPressed && kb.insertKey.isPressed)
        {
            ToggleDebugMenu();
            return;
        }

        // No relevant input pressed -> reset state
        buttonPressed = false;
    }
}
