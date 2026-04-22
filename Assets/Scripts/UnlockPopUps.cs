using UnityEngine;

public class UnlockPopUps : MonoBehaviour
{
    [SerializeField] private GameObject silverPopUp;
    [SerializeField] private GameObject goldPopUp;

    private void Start()
    {
        silverPopUp.SetActive(false);
        goldPopUp.SetActive(false);
    }

    public void ShowUnlockPopUp(string unlockName)
    {
        if (unlockName == "Silver")
        {
            silverPopUp.SetActive(true);
        }
        if (unlockName == "Gold")
        {
            goldPopUp.SetActive(true);
        }
    }
}
