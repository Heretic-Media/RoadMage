using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class VictoryVFXBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI youWinText;
    [SerializeField] private TextMeshProUGUI FinalScoreText;
    [SerializeField] private TextMeshProUGUI ScoreText;

    private float startTime = 9999999;

    void Awake()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = true;
        startTime = Time.time;
        GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().Pause();
    }

    private void FixedUpdate()
    {
        transform.localScale += Vector3.one * Time.fixedDeltaTime * 0.5f;
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
            ScoreText.enabled = true;

            string fmt = "000000";
            int score = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>().GetScore();
            ScoreText.text = score.ToString(fmt);
        }
        else if (Time.time > 6 + startTime)
        {
            // final score
            FinalScoreText.enabled = true;

        }
        else if (Time.time > 3 + startTime)
        {
            // you win!
            youWinText.enabled = true;
        }
    }
}
