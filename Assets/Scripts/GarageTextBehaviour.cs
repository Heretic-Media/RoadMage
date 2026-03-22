using TMPro;
using UnityEngine;

public class GarageTextBehaviour : MonoBehaviour
{
    private int garagesDone = 0;
    [SerializeField] private int garagesToDo = 4;

    public void AddGarageScore()
    {
        garagesDone++;
        GetComponent<TextMeshProUGUI>().text = garagesDone.ToString() + "/" + garagesToDo.ToString();
        if (garagesDone >= garagesToDo)
        {
            GameObject.FindGameObjectWithTag("Goal").GetComponent<PortalBehaviour>().UnlockDoor();
        }
    }

    public int GetGaragesDone()
    {
        return garagesDone;
    }
}
