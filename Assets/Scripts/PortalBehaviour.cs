using TMPro;
using UnityEngine;

public class PortalBehaviour : MonoBehaviour
{
    [SerializeField] private BoxCollider trigger;
    [SerializeField] private GameObject portal;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GameObject victoryPrefab;
    [SerializeField] private GameObject enemiesObject;
    [SerializeField] private UIBarBehaviour infestationBar;
    [SerializeField] private GameObject infestationText;
    [SerializeField] private Canvas infestationCanvas;

    private bool inited = false;
    private int enemiesNum = 8;

    private void initiateWin()
    {
        infestationCanvas.enabled = false;
        GameObject victoryVFX = Instantiate(victoryPrefab);
        victoryVFX.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.CompareTag("Player") || collision.isTrigger)
            return;

        initiateWin();
    }

    public void InitDoor()
    {
        portal.SetActive(true);
        enemiesObject.SetActive(true);
        spawner.enabled = true;
        inited = true;
        infestationCanvas.enabled = true;
        enemiesNum = GetEnemies();
    }

    public void UnlockDoor()
    {
        // if you're putting in an animation for the door opening, make it play here

        trigger.enabled = true;

    }

    int GetEnemies()
    {
        return enemiesObject.transform.childCount;
    }

    void FixedUpdate()
    {
        if (inited)
        {
            if (GetEnemies() == 0)
            {
                UnlockDoor();
            }

            if (infestationBar != null)
            {
                infestationBar.UpdateBar((float)GetEnemies() / (float)enemiesNum);
            }

            TextMeshProUGUI tmp = infestationText.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.SetText(GetEnemies().ToString() + " / " + enemiesNum.ToString());
            }
        }

    }
}
