using UnityEditor.Experimental.GraphView;
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
        blankIcons[iconSpaceNeeded - 1].SetActive(false);
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
                    fireyDriftIconActive = true;
                    iconSpaceNeeded++;
                    activateAbilityIcon(1); // Activate the second icon for the drift ability
                    return;
                }
                if (player.transform.GetChild(i).name == "ForwardAbility(Clone)" && !kineticBlastIconActive)
                {
                    kineticBlastIconActive = true;
                    iconSpaceNeeded++;
                    activateAbilityIcon(3); // Activate the fourth icon for kinetic blast ability
                    return;
                }

            }
        }
    }
        // Update is called once per frame
        void Update()
    {
        isAbilityActive();
    }
}
