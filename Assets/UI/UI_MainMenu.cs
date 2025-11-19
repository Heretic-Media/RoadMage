using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("ALPHA");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");

        Application.Quit();
    }
}
