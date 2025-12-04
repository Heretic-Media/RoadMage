using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    public List<EnemyBehaviour> ChasingEnemies = new List<EnemyBehaviour>();

    [Tooltip("Maximum number of enemies allowed to chase/attack at once.")]
    public int maxChasingEnemies = 10;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public bool CanRegisterChasing()
    {
        return ChasingEnemies.Count < maxChasingEnemies;
    }

    public void RegisterChasing(EnemyBehaviour enemy)
    {
        if (!ChasingEnemies.Contains(enemy) && CanRegisterChasing())
            ChasingEnemies.Add(enemy);
    }

    public void UnregisterChasing(EnemyBehaviour enemy)
    {
        ChasingEnemies.Remove(enemy);
    }
}