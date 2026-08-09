using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public GameObject particlePrefab;

    public int particleCount = 30;

    
    public Vector2 spawnArea = new Vector2(800, 400);

    void Start()
    {
        SpawnAllParticles();
    }

    void SpawnAllParticles()
    {
        for (int i = 0; i < particleCount; i++)
        {
            SpawnParticle(i);
        }
    }

    void SpawnParticle(int index)
    {
        GameObject particle = Instantiate(particlePrefab, transform);

        
        particle.transform.localScale = Vector3.one;

        RectTransform rect = particle.GetComponent<RectTransform>();

        
        float x = Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
        float y = Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);

        rect.anchoredPosition = new Vector2(x, y);

        // Debug.Log("Spawned particle " + index);
    }
}