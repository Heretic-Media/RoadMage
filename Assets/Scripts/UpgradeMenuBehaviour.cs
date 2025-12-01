using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeMenuBehaviour : MonoBehaviour
{
    public GameObject defaultOption;
    [SerializeField] EventSystem eventSystem;
    
    public void Unpause(GameObject upgradePrefab)
    {
        eventSystem.SetSelectedGameObject(defaultOption);
        Time.timeScale = 1.0f;
        GetComponent<Canvas>().enabled = false;
    }
}
