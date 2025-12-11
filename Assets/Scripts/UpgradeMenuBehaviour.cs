using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeMenuBehaviour : MonoBehaviour
{
    [SerializeField] public GameObject defaultOption;
    [SerializeField] public EventSystem eventSystem;
    

    public void Unpause(GameObject upgradePrefab)
    {
        
        Time.timeScale = 1.0f;
        UpgradePlayer(upgradePrefab);
        GetComponent<Canvas>().enabled = false;
    }

    public void UpgradePlayer(GameObject prefabToUse)
    {
        
        GameObject newUpgrade = Instantiate(prefabToUse);
        newUpgrade.transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform, false);
    }

    public void init()
    {
        eventSystem.SetSelectedGameObject(defaultOption);
    }
}
