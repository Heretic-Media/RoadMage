using UnityEngine;
using UnityEngine.SceneManagement;

public class GarageBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject enemiesObject;
    [SerializeField] private BoxCollider physical;
    [SerializeField] private BoxCollider trigger;
    private Canvas upgradeMenu;


    int GetEnemies()
    {
        return enemiesObject.transform.childCount;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        upgradeMenu = GameObject.FindGameObjectWithTag("UpgradeUI").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetEnemies() == 0)
        {
            physical.enabled = false;
            trigger.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (!collision.gameObject.CompareTag("Player"))
            return;

        TopDownCarController mScript = collision.gameObject.GetComponent<TopDownCarController>();

        AccessUpgradeMenu();
        Destroy(gameObject);
    }

    private void AccessUpgradeMenu()
    {
        Time.timeScale = 0.0f;
        upgradeMenu.enabled = true;
    }
}
