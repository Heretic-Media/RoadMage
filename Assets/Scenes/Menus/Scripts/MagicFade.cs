using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;


 public class MagicFade : MonoBehaviour
{
    public Image fadeImage;

    public void FadeTo(string StartGame)
    {
        StartCoroutine(Fade(StartGame));
    }

    System.Collections.IEnumerator Fade(string StartGame)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 0.8f;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }
        SceneManager.LoadScene(StartGame);
    }
}
