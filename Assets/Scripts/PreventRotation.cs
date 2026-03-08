using UnityEngine;

public class PreventRotation : MonoBehaviour
{ 
    [SerializeField] private bool useGlobalRotation = true;

    [SerializeField] private bool lockX = true;
    [SerializeField] private bool lockY = true;
    [SerializeField] private bool lockZ = true;

    private Vector3 initialRotation;

    void Awake()
    {
        initialRotation = transform.localEulerAngles;
    }

    void LateUpdate()
    {
        Vector3 newRotation = transform.localEulerAngles;

        if (lockX) newRotation.x = initialRotation.x;
        if (lockY) newRotation.y = initialRotation.y;
        if (lockZ) newRotation.z = initialRotation.z;

        if (useGlobalRotation)
        {
            transform.eulerAngles = newRotation;
        }
        else
        {
            transform.localEulerAngles = newRotation;
        }
    }
}
