using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class UpgradeMenuBehaviour : MonoBehaviour
{
    [SerializeField] GameObject[] optionButtons;
    int[] options;
    [SerializeField] EventSystem eventSystem;

    [SerializeField] GameObject canvas;

    [SerializeField] GameObject[] upgradePrefabs;
    List<int> availableUpgrades = new List<int>();

    [Space]
    [SerializeField] GameObject[] upgradeObjects;

    private void Start()
    {
        options = new int[optionButtons.Length];
        for (int i = 0; i < upgradePrefabs.Length; i++)
        {
            availableUpgrades.Add(i);
        }

        upgradeObjects = new GameObject[upgradePrefabs.Length];
    }

    public void Unpause(int buttonIndex)
    {
        Init();
        GameObject.FindGameObjectWithTag("PauseManager").GetComponent<PauseManager>().UpgradeMenuPause(false);
        UpgradePlayer(options[buttonIndex]);
        canvas.SetActive(false);
    }

    public void Pause()
    {
        GameObject.FindGameObjectWithTag("PauseManager").GetComponent<PauseManager>().UpgradeMenuPause(true);
        List<int> selectedupgrades = new List<int>();
        for (int i = 0; i < optionButtons.Length; i++)
        {
            options[i] = SelectUpgrades();
            selectedupgrades.Add(options[i]);
            availableUpgrades.Remove(options[i]);

            if (options[i] < 0)
            {
                //optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = 
                //    "Skip upgrade";
                TextMeshProUGUI[] abilityTexts = optionButtons[i].GetComponentsInChildren<TextMeshProUGUI>();
                abilityTexts[0].text = "Skip upgrade";
                abilityTexts[1].text = "Skips this upgrade";
            }
            else
            {
                AbilityDesc abilityDescription = upgradePrefabs[options[i]].GetComponent<AbilityDesc>();
                TextMeshProUGUI[] abilityTexts = optionButtons[i].GetComponentsInChildren<TextMeshProUGUI>();

                if (upgradeObjects[options[i]] != null)
                {
                    UpgradeAbility upgradeAbility = upgradeObjects[options[i]].GetComponent<UpgradeAbility>();

                    if (upgradeAbility != null)
                    {
                        abilityTexts[0].text = abilityDescription.abilityName + " Upgrade Level: " + (upgradeAbility.level + 1).ToString();
                        abilityTexts[1].text = abilityDescription.abilityDesc;
                    }
                    else
                    {
                        abilityTexts[0].text = abilityDescription.abilityName;
                        abilityTexts[1].text = abilityDescription.abilityDesc;
                    }
                }
                else
                {
                    abilityTexts[0].text = abilityDescription.abilityName;
                    abilityTexts[1].text = abilityDescription.abilityDesc;
                }
                //optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = 
                //    upgradePrefabs[options[i]].GetComponent<AbilityDesc>().abilityName;
            }
        }

        foreach (var item in selectedupgrades)
        {
            availableUpgrades.Add(item);
        }
        selectedupgrades.Clear();

        canvas.SetActive(true);
        Init();
    }

    void UpgradePlayer(int prefabIndex)
    {
        if (prefabIndex < 0)
        {
            return;
        }

        GameObject player = FindFirstObjectByType<Player>().gameObject;

        bool instantiated = false;
        if (upgradeObjects[prefabIndex] == null)
        {
            upgradeObjects[prefabIndex] = Instantiate(upgradePrefabs[prefabIndex], player.transform);
            instantiated = true;
        }

        UpgradeAbility upgradeAbility = upgradeObjects[prefabIndex].GetComponent<UpgradeAbility>();

        if (upgradeAbility != null)
        {
            if (upgradeAbility.level == upgradeAbility.upgrades)
            {
                availableUpgrades.Remove(prefabIndex);
            }
            else if (!instantiated && upgradeAbility.level < upgradeAbility.upgrades)
            {
                upgradeAbility.Upgrade();
            }
        }
        else
        {
            availableUpgrades.Remove(prefabIndex);
        }

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
