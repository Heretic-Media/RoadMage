using UnityEngine;
using UnityEngine.UI;

public class WinScreenManager : MonoBehaviour
{
    [SerializeField] private Image starFillA;
    [SerializeField] private Image starFillB;
    [SerializeField] private Image starFillC;

    [SerializeField] private GameObject winScreen;

    public void ChangeWinScreenVisibility(bool visible)
    {
        winScreen.SetActive(visible);
    }

    public void ChangeStarFill(int amount)
    {
        starFillA.enabled = false;
        starFillB.enabled = false;
        starFillC.enabled = false;

        if (amount >= 1)
        {
            starFillA.enabled = true;
        }
        if (amount >= 2)
        {
            starFillB.enabled = true;
        }
        if (amount >= 3)
        {
            starFillC.enabled = true;
        }
    }
}
