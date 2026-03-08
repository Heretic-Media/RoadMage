using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("ALPHA with assets");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");

        Application.Quit();
    }

    public void CharacterCustomisation()
    {
        SceneManager.LoadScene("CharacterCustomisation");
    }

    public void Transition()
    {
        SceneManager.LoadScene("PortalTransition");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
