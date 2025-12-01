using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeMenuBehaviour : MonoBehaviour
{
    [SerializeField] GameObject defaultOption;
    [SerializeField] EventSystem eventSystem;
    
    public void Unpause(GameObject upgradePrefab)
    {
        eventSystem.SetSelectedGameObject(defaultOption);
        Time.timeScale = 1.0f;
        UpgradePlayer(upgradePrefab);
        GetComponent<Canvas>().enabled = false;
    }

    public void UpgradePlayer(GameObject prefabToUse)
    {
        
        GameObject newUpgrade = Instantiate(prefabToUse);
        newUpgrade.transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform, false);
    }
}
