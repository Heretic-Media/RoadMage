using UnityEngine;

public class UpgradeMenuBehaviour : MonoBehaviour
{
    public void Unpause()
    {
        Time.timeScale = 1.0f;
        GetComponent<Canvas>().enabled = false;
    }
}
