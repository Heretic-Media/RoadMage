using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public float amount = 0.4f;

    void Update()
    {
        float x = (Input.mousePosition.x / Screen.width - 0.5f) * amount;
        float y = (Input.mousePosition.y / Screen.height - 0.5f) * amount;
        transform.localRotation = Quaternion.Euler(y, -x, 0);
    }
}
