using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GarageBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject enemiesObject;
    [SerializeField] private GameObject infestedIndicator;
    [SerializeField] private GameObject infestationText;
    [SerializeField] private BoxCollider[] physical;
    [SerializeField] private BoxCollider trigger;
    [SerializeField] private UIBarBehaviour infestationBar;
    private UpgradeMenuBehaviour upgradeMenu;
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
        upgradeMenu = FindFirstObjectByType<UpgradeMenuBehaviour>();
        enemiesNum = enemiesObject.transform.childCount;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (exploding)
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
        else if (infestedIndicator != null)
        {
            infestedIndicator.transform.localScale = new Vector3(Mathf.Abs(Mathf.Sin(Time.time)), 0.01f, Mathf.Abs(Mathf.Sin(Time.time)));
        }

        if (infestationBar != null)
        {
            infestationBar.UpdateBar((float)GetEnemies() / (float)enemiesNum);
        }

        TextMeshProUGUI tmp = infestationText.GetComponent<TextMeshProUGUI>();
        if(tmp != null) 
        {
            tmp.SetText(GetEnemies().ToString() + " / " + enemiesNum.ToString());
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.CompareTag("Player") || exploding || collision.isTrigger)
            return;

        TopDownCarController mScript = collision.gameObject.GetComponent<TopDownCarController>();

        upgradeMenu.Pause();
        exploding = true;
        trigger.enabled = false;

    }

    private void AccessUpgradeMenu()
    {
        upgradeMenu.Pause();
    }
}