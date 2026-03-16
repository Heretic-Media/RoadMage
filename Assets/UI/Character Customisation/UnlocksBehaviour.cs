using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class UnlocksBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private GameObject[] unlockButtons;
    //public static string[] unlockNames;

    private void Start()
    {
        if (UnlocksForLevel.unlockNames == null)
        {
            unlockPanel.SetActive(false);
        }
        else
        {
            unlockPanel.SetActive(true);
        }

    }

    public void resetUnlocks()
    {
        for (int i = 0; i < unlockButtons.Length; i++)
        {
            unlockButtons[i].SetActive(false);
        }

        UnlocksForLevel.unlockNames = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (UnlocksForLevel.unlockNames != null && unlockButtons != null)
        {
            foreach (var unlock in UnlocksForLevel.unlockNames)
            {
                if (unlock == "Silver")
                {
                    unlockButtons[0].SetActive(true);
                }
                if (unlock == "Gold")
                {
                    unlockButtons[1].SetActive(true);
                }
            }
        }
    }
}
