using Unity.Mathematics;
using UnityEngine;

public class billboard : MonoBehaviour
{
    // Script sourced from about game making available from here: https://www.youtube.com/watch?v=GuWEXBeHEy8

    [SerializeField] private Camera mainCamera;

    private void Reset()
    {
        mainCamera = Camera.main;
    }

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        Quaternion rotation = mainCamera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }
}