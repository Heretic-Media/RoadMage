using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryVFXBehaviour : MonoBehaviour
{

    private float startTime = 9999999;
    private WinScreenManager winScreenManager;

    void Awake()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = true;
        startTime = Time.time;
        GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().Pause();

        winScreenManager = GameObject.FindGameObjectWithTag("WinScreenManager").GetComponent<WinScreenManager>();
    }

    private void FixedUpdate()
    {
        transform.localScale += Vector3.one * Time.fixedDeltaTime * 2f;
        GameObject.FindGameObjectWithTag("Player").transform.position += Vector3.up * Time.fixedDeltaTime;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().health = 100;
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.transform.position = Vector3.up * 999999;
        }

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraBehaviour>().Shake(Time.fixedDeltaTime * 2, (Time.time - startTime) * 0.01f);

        if (Time.time > 15 + startTime)
        {
            // back to menu
            SceneManager.LoadScene("MainMenu");
        }
        else if (Time.time > 9 + startTime)
        {
            // Star fill 3
            winScreenManager.ChangeStarFill(3);
        }
        else if (Time.time > 8 + startTime)
        {
            // Star fill 2
            winScreenManager.ChangeStarFill(2);
        }
        else if (Time.time > 7 + startTime)
        {
            // Star fill 1
            winScreenManager.ChangeStarFill(1);
        }
        else if (Time.time > 6 + startTime)
        {
            // Score Text
            winScreenManager.EneableScoreText();
        }
        else if (Time.time > 3 + startTime)
        {
            // Victory screen
            winScreenManager.ChangeWinScreenVisibility(true);
        }
    }
}
