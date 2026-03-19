using UnityEngine;

public class UnlocksForLevel : MonoBehaviour
{
    public static string[] unlockNames;


    public void addUnlockedItem(string name)
    {
        if (unlockNames == null)
        {
            unlockNames = new[] { name };
            return;
        }

        System.Array.Resize(ref unlockNames, unlockNames.Length + 1);
        unlockNames[unlockNames.Length - 1] = name;
    }

}
