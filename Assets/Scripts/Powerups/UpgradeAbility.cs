using UnityEngine;

public class UpgradeAbility : MonoBehaviour
{
    public int upgrades = 0;
    public int level = 0;

    public bool upgraded = false;

    public void Upgrade() 
    {
        if (level < upgrades) 
        {
            level++;
            upgraded = true;
        }
    }
}
