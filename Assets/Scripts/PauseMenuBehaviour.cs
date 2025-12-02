using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseMenuBehaviour : MonoBehaviour
{
    [SerializeField] GameObject defaultOption;
    [SerializeField] EventSystem eventSystem;
    private Canvas pauseMenu;

    void Awake()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseUI").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AccessPauseMenu()
    {
        Time.timeScale = 0.0f;
        pauseMenu.enabled = true;
    }

    public void Unpause(GameObject upgradePrefab)
    {
        eventSystem.SetSelectedGameObject(defaultOption);
        Time.timeScale = 1.0f;
        GetComponent<Canvas>().enabled = false;
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit()
    {
        Application.Quit();
    }

}
