using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("The prefab that the spawner will spawn.")]
    [SerializeField] private GameObject enemyPrefab;

    [Tooltip("The maximum number of enemies that can exist before we stop spawning them.")]
    [SerializeField] private int maxEnemies = 100;

    [Tooltip("Time in seconds between enemy spawn attempts.")]
    [SerializeField] private float spawnDelay = 1.0f;

    private float timer = 1f;

    private void SpawnEnemy()
    {
        if (GetEnemyCount() < maxEnemies)
        {
            Instantiate(enemyPrefab, transform.position + new Vector3(1.5f, 0.52f, 0), Quaternion.identity);
        }
    }

    private int GetEnemyCount()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        return allEnemies.Length;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = spawnDelay;
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer > spawnDelay) 
        {
            timer -= spawnDelay;
            SpawnEnemy();
        }
    }
}
