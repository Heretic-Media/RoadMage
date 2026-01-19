using UnityEngine;
using UnityEngine.SceneManagement;

public class GarageBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject enemiesObject;
    [SerializeField] private GameObject infestedIndicator;
    [SerializeField] private BoxCollider[] physical;
    [SerializeField] private BoxCollider trigger;
    [SerializeField] private UIBarBehaviour infestationBar;
    private GameObject upgradeMenu;
    private int enemiesNum;
    private bool exploding = false;
    private float explodingTimer = 1f;


    int GetEnemies()
    {
        return enemiesObject.transform.childCount;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        upgradeMenu = GameObject.FindGameObjectWithTag("UpgradeUI");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetEnemies() == 0)
        {
            explodingTimer -= Time.fixedDeltaTime;
            if (explodingTimer <= 0)
            {
                Destroy(transform.parent.gameObject);
            }
        }
        
        if (GetEnemies() == 0 && !exploding)
        {
            foreach (BoxCollider col in physical)
            {
                col.enabled = false;
                if (infestedIndicator != null)
                {
                    infestedIndicator.SetActive(false);
                }
            }
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
        upgradeMenu.SetActive(true);
    }
}
