using UnityEngine;

public class PreventRotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Quaternion startRotation = Quaternion.identity;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = startRotation;
    }
}
