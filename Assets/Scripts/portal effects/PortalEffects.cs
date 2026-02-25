using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalEffects : MonoBehaviour
{
    private float timer;
    public float maxDelay;
    public GameObject transitionScreen;

    private void Start()
    {
        transitionScreen.SetActive(false);
    }

    private void transitionScene()
    {
        if (timer < maxDelay)
        {
            timer += Time.deltaTime;
        }
        else
        {
            transitionScreen.SetActive(true);
            SceneManager.LoadScene("ALPHA with assets");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        transitionScene();
    }
}
