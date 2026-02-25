using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UpgradeMenuBehaviour : MonoBehaviour
{
    [SerializeField] GameObject[] optionButtons;
    int[] options;
    [SerializeField] EventSystem eventSystem;

    [SerializeField] GameObject[] upgradePrefabs;
    List<int> availableUpgrades = new List<int>();

    private void Start()
    {
        options = new int[optionButtons.Length];
        for (int i = 0; i < upgradePrefabs.Length; i++)
        {
            availableUpgrades.Add(i);
        }
    }

    public void Unpause(int buttonIndex)
    {
        Init();
        Time.timeScale = 1.0f;
        UpgradePlayer(options[buttonIndex]);
        GetComponent<Canvas>().enabled = false;
    }

    public void Pause()
    {
        Init();
        Time.timeScale = 0.0f;
        List<int> selectedupgrades = new List<int>();
        for (int i = 0; i < optionButtons.Length; i++)
        {
            options[i] = SelectUpgrades();
            selectedupgrades.Add(options[i]);
            availableUpgrades.Remove(options[i]);

            if (options[i] < 0)
            {
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = 
                    "Skip upgrade";
            }
            else
            {
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = 
                    upgradePrefabs[options[i]].name;
            }
        }

        foreach (var item in selectedupgrades)
        {
            availableUpgrades.Add(item);
        }
        selectedupgrades.Clear();

        GetComponent<Canvas>().enabled = true;
    }

    public void UpgradePlayer(int prefabIndex)
    {
        if (prefabIndex < 0)
        {
            return;
        }

        GameObject player = FindFirstObjectByType<Player>().gameObject;
        Instantiate(upgradePrefabs[prefabIndex], player.transform);

        availableUpgrades.Remove(prefabIndex);

        //bool alreadyUpgraded = false;

        //for (int i = 0; i < player.transform.childCount; i++)
        //{
        //    if (prefabToUse.name == player.transform.GetChild(i).name.Replace("(Clone)", ""))
        //    {
        //        alreadyUpgraded = true;
        //        break;
        //    }
        //}

        //if (!alreadyUpgraded)
        //{
        //    GameObject newUpgrade = Instantiate(prefabToUse, player.transform);
        //}
    }

    void Init()
    {
        eventSystem.SetSelectedGameObject(optionButtons[0]);
    }

    int SelectUpgrades()
    {
        int output = -1;

        if (availableUpgrades.Count > 0)
        {
            int index = Random.Range(0, availableUpgrades.Count);
            output = availableUpgrades[index];
        }

        return output;
    }
}
