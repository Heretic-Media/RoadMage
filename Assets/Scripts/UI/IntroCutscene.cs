using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{
    private float timer;
    public float maxDelay;
    public GameObject transitionScreen;

    private void Start()
    {
    }

    private void transitionScene()
    {
        if (timer < maxDelay)
        {
            timer += Time.deltaTime;
        }
        else
        {
            SceneManager.LoadScene("PortalTransition");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        transitionScene();
    }
}
