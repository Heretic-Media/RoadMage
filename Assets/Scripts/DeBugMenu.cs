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
}
