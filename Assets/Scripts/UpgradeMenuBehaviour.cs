using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeMenuBehaviour : MonoBehaviour
{
    [SerializeField] GameObject defaultOption;
    [SerializeField] EventSystem eventSystem;
    
    public void Unpause(GameObject upgradePrefab)
    {
        if (GetComponent<Canvas>().enabled == true)
        {
            eventSystem.SetSelectedGameObject(defaultOption);
            Time.timeScale = 1.0f;
            UpgradePlayer(upgradePrefab);
            GetComponent<Canvas>().enabled = false;
        }
    }

    public void UpgradePlayer(GameObject prefabToUse)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        bool alreadyUpgraded = false;

        for (int i = 0; i < player.transform.childCount; i++)
        {
            if (prefabToUse.name == player.transform.GetChild(i).name.Replace("(Clone)", ""))
            {
                alreadyUpgraded = true;
                break;
            }
        }

        if (!alreadyUpgraded)
        {
            GameObject newUpgrade = Instantiate(prefabToUse, player.transform);
        }
    }

    public void init()
    {
        eventSystem.SetSelectedGameObject(defaultOption);
    }
}
