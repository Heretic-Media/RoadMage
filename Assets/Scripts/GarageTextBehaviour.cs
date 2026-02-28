using TMPro;
using UnityEngine;

public class GarageTextBehaviour : MonoBehaviour
{
    private int garagesDone = 0;

    public void AddGarageScore()
    {
        garagesDone++;
        GetComponent<TextMeshProUGUI>().text = garagesDone.ToString() + "/4";
    }
}
