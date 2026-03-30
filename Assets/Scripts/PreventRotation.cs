using UnityEngine;

public class PreventRotation : MonoBehaviour
{
    [SerializeField] private bool useGlobalRotation = true;

    [SerializeField] private bool lockX = true;
    [SerializeField] private bool lockY = true;
    [SerializeField] private bool lockZ = true;

    private Vector3 initialLocalEuler;
    private Vector3 initialWorldEuler;

    void Awake()
    {
        initialLocalEuler = transform.localEulerAngles;
        initialWorldEuler = transform.eulerAngles;
    }

    void LateUpdate()
    {
        if (useGlobalRotation)
        {
            Vector3 newRotation = transform.localEulerAngles;

            if (lockX) newRotation.x = initialLocalEuler.x;
            if (lockY) newRotation.y = initialLocalEuler.y;
            if (lockZ) newRotation.z = initialLocalEuler.z;

            transform.eulerAngles = newRotation;
        }
        else
        {
            Vector3 newRotation = transform.localEulerAngles;

            if (lockX) newRotation.x = initialWorldEuler.x;
            if (lockY) newRotation.y = initialWorldEuler.y;
            if (lockZ) newRotation.z = initialWorldEuler.z;

            transform.localEulerAngles = newRotation;
        }
    }
}
