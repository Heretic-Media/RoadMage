using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    public Text starText; // Reference to star count text in UI

    void Start()
    {
        CurrencyManager.LoadFromSave();
        if (starText != null)
        {
            starText.text = CurrencyManager.stars.ToString();
        }
    }

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
