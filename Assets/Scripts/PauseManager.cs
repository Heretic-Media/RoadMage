using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool upgradeMenuPaused = false;
    private bool pauseMenuPaused = false;

    public void RefreshPausing()
    {
        if (upgradeMenuPaused || pauseMenuPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void UpgradeMenuPause(bool paused)
    {
        upgradeMenuPaused = paused;

        RefreshPausing();
    }

    public void PauseMenuPause(bool paused)
    {
        pauseMenuPaused = paused;

        RefreshPausing();
    }
}
