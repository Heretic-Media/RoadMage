using System.Collections;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool upgradeMenuPaused = false;
    private bool pauseMenuPaused = false;
    private bool hitStopPaused = false;

    private ShakeSettingManager shakeSettingManager;

    private void Awake()
    {
        shakeSettingManager = GameObject.FindGameObjectWithTag("ShakeSettingsManager").GetComponent<ShakeSettingManager>();
    }

    public void RefreshPausing()
    {
        if (upgradeMenuPaused || pauseMenuPaused || hitStopPaused)
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

    public void InitiateHitStop(float duration)
    {   
        StartCoroutine(HitStop(Mathf.Min(duration, shakeSettingManager.shakeStrength * 0.075f)));
    }

    IEnumerator HitStop(float duration)
    {
        hitStopPaused = true;
        RefreshPausing();

        yield return new WaitForSecondsRealtime(shakeSettingManager.shakeStrength * duration);

        hitStopPaused = false;
        RefreshPausing();
    }
}
