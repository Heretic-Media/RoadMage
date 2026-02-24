using UnityEngine;

public class PreventRotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Quaternion startRotation = Quaternion.identity;

    [SerializeField] private bool useGlobalStartRotation = false;

    void Start()
    {
        startRotation = transform.localRotation;

        //if (useGlobalStartRotation)
        //{
        //    startRotation = transform.rotation;
        //}
        //else
        //{
            
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (useGlobalStartRotation)
        {
            transform.rotation = startRotation;
        }
        else
        {
            transform.localRotation = startRotation;
        }
    }
}
