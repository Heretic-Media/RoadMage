using UnityEngine;
using UnityEngine.InputSystem;
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
            SceneManager.LoadScene("MainMenu");
        }
    }

    // Update is called once per frame
    void Update()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        if (kb != null && kb.spaceKey.wasPressedThisFrame || gp != null && gp.aButton.wasPressedThisFrame)
        {
            maxDelay = 0;
            return;
        }
        transitionScene();
    }
}
