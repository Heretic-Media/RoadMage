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

        if (useGlobalRotation)
        {
            transform.eulerAngles = newRotation;
        }
        else
        {
            transform.localEulerAngles = newRotation;
        }

        if (lockX) transform.eulerAngles = new Vector3(initialRotation.x, transform.eulerAngles.y, transform.eulerAngles.z);
        if (lockY) transform.eulerAngles = new Vector3(transform.eulerAngles.x, initialRotation.y, transform.eulerAngles.z);
        if (lockZ) transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, initialRotation.z);
    }
}
