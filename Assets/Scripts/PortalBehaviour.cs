using UnityEngine;

public class PortalBehaviour : MonoBehaviour
{
    [SerializeField] private BoxCollider trigger;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GameObject victoryPrefab;

    private void initiateWin()
    {
        GameObject victoryVFX = Instantiate(victoryPrefab);
        victoryVFX.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.CompareTag("Player") || collision.isTrigger)
            return;

        initiateWin();
    }

    public void UnlockDoor()
    {
        trigger.enabled = true;
        spawner.enabled = true;
    }
}
