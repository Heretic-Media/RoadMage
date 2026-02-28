using UnityEngine;
using UnityEngine.UI;

public class UIAbilityBar : MonoBehaviour
{
    public Player player;
    public GameObject[] abilityIcons;
    public GameObject[] blankIcons;
    private int iconSpaceNeeded = 0;
    private bool fireyDriftIconActive = false;
    private bool kineticBlastIconActive = false;
    // private bool [name here]IconActive = false; // Add a boolean for each new ability icon


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < abilityIcons.Length; i++)
        {
            abilityIcons[i].SetActive(false);

        }
        abilityIcons[0].SetActive(true);
        abilityIcons[2].SetActive(true);
    }

    private void activateAbilityIcon(int ability)
    {
        if (iconSpaceNeeded > 0)
        {
          blankIcons[iconSpaceNeeded - 1].SetActive(false);
        }
        abilityIcons[ability].SetActive(true);
    }

    private void isAbilityActive()
    {
        if (player.transform.childCount > 0)
        {
            for (int i = 0; i < player.transform.childCount; i++)
            {
                if (player.transform.GetChild(i).name == "DriftAbility(Clone)" && !fireyDriftIconActive)
                {
                    abilityIcons[0].SetActive(false); // Deactivate the default icon for the drift ability
                    fireyDriftIconActive = true;
                    activateAbilityIcon(1); // Activate the icon for the drift ability
                    return;
                }

                if (player.transform.GetChild(i).name == "ForwardAbility(Clone)" && !kineticBlastIconActive)
                {
                    abilityIcons[2].SetActive(false); // Deactivate the default icon for the handbrake
                    kineticBlastIconActive = true;
                    activateAbilityIcon(3); // Activate the icon for kinetic blast ability
                    return;
                }

                //if (player.transform.GetChild(i).name == "[name here](Clone)" && ![name here]IconActive)
                //{
                    //[name here]IconActive = true;
                    //iconSpaceNeeded++;
                    //activateAbilityIcon(iconNumberInList); // Activate the icon for [name here] ability
                    //return;
                //}

            }
        }
    }
        // Update is called once per frame
        void Update()
    {
        isAbilityActive();
    }
}
