using UnityEngine;
using UnityEngine.UI;

public class UIAbilityBar : MonoBehaviour
{
    public Player player;
    public GameObject[] abilityIcons;
    public GameObject[] cooldownIcons;
    private bool fireyDriftIconActive = false;
    private bool kineticBlastIconActive = false;
    private bool reFuelIconActive = false;
    private bool iceVortexIconActive = false;

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

                if (player.transform.GetChild(i).name == "HealAbility(Clone)" && !reFuelIconActive)
                {
                reFuelIconActive = true;
                activateAbilityIcon(4); // Activate the icon for [name here] ability
                return;
                }

                if (player.transform.GetChild(i).name == "VortexAbility(Clone)" && !iceVortexIconActive)
                {
                    iceVortexIconActive = true;
                    activateAbilityIcon(5); // Activate the icon for [name here] ability
                    return;
                }

                //if (player.transform.GetChild(i).name == "[name here](Clone)" && ![name here]IconActive)
                //{
                //[name here]IconActive = true;
                //activateAbilityIcon(iconNumberInList); // Activate the icon for [name here] ability
                //return;
                //}

            }
        }
    }

    private void isAbilityOnCooldown()
    {
        if (player.transform.childCount > 0)
        {
            for (int i = 0; i < player.transform.childCount; i++)
            {

                //if (player.transform.GetChild(i).name == "DriftAbility(Clone)" && fireyDriftIconActive)
                //{
                //    if (player.transform.GetChild(i).GetComponent<DriftAbility>().driftDelayTime > 0)
                //    {
                //        cooldownIcons[0].SetActive(true);
                //        abilityIcons[1].SetActive(false);
                //    }
                //    else
                //    {
                //        cooldownIcons[0].SetActive(false);
                //        abilityIcons[1].SetActive(true);
                //    }
                //}

                if (player.transform.GetChild(i).name == "forwardability(clone)" && kineticBlastIconActive)
                {
                    if (player.transform.GetChild(i).GetComponent<ForwardAbility>().attackCooldown > 0)
                    {
                        cooldownIcons[1].SetActive(true);
                        abilityIcons[3].SetActive(false);
                    }
                    else
                    {
                        cooldownIcons[1].SetActive(false);
                        abilityIcons[3].SetActive(true);
                    }
                }


                if (player.transform.GetChild(i).name == "HealAbility(Clone)" && reFuelIconActive)
                {
                    if (player.transform.GetChild(i).GetComponent<HealAbility>().healOnCooldown)
                    {
                        cooldownIcons[2].SetActive(true);
                        abilityIcons[4].SetActive(false);
                    }
                    else
                    {
                        cooldownIcons[0].SetActive(false);
                        abilityIcons[4].SetActive(true);
                    }
                }

                if (player.transform.GetChild(i).name == "VortexAbility(Clone)" && iceVortexIconActive)
                {
                    if (player.transform.GetChild(i).GetComponent<VortexAbility>().vortexOnCooldown)
                    {
                        cooldownIcons[3].SetActive(true);
                        abilityIcons[5].SetActive(false);
                    }
                    else
                    {
                        cooldownIcons[1].SetActive(false);
                        abilityIcons[5].SetActive(true);
                    }
                }
            }
        }

        else
        {
            return;
        }
    }

        // Update is called once per frame
        void Update()
    {
        isAbilityActive();
        isAbilityOnCooldown();
    }
}
