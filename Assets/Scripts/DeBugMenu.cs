using UnityEngine;

public class DeBugMenu : MonoBehaviour
{
    public void KillAllSLimes()
    {         GameObject[] slimes = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject slime in slimes)
        {
            Destroy(slime);
        }
    }

    public void GiveAllUnlocks()
    {
        GameObject.FindGameObjectWithTag("UnlockManager").GetComponent<UnlocksForLevel>().addUnlockedItem("Gold");
        GameObject.FindGameObjectWithTag("UnlockManager").GetComponent<UnlocksForLevel>().addUnlockedItem("Silver");

    }
}
