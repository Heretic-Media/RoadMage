using UnityEngine;

public class MenuCursor : MonoBehaviour
{
    void Start()
    {
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnDestroy()
    {
        Cursor.visible = true;
    }
}
