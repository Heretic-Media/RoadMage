using UnityEngine;
using UnityEngine.InputSystem;
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
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        if (kb != null && kb.spaceKey.isPressed || gp != null && gp.aButton.isPressed)
        {
            maxDelay = 0;
            return;
        }
        transitionScene();
    }
}
