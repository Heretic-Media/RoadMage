using UnityEngine;
using UnityEngine.SceneManagement;

public class AlphaGarage : MonoBehaviour
{
    [SerializeField] private GameObject enemiesObject;
    [SerializeField] private BoxCollider physical;
    [SerializeField] private BoxCollider trigger;
    
    int GetEnemies()
    {
        return enemiesObject.transform.childCount;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

        if (mScript.enableDriftProjectiles)
        {
            SceneManager.LoadScene("WinScreen");
        }

        mScript.enableDriftProjectiles = true;
        Destroy(gameObject);
    }
}
