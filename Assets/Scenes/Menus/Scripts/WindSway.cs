using UnityEngine;

public class WindSway : MonoBehaviour
{
    [Header("Wind Settings")]
    public float swayAmount = 3f;     
    public float swaySpeed = 0.5f;    

    [Header("Floating Movement")]
    public float floatAmount = 0.2f;  
    public float floatSpeed = 0.3f;   

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float floatOffset = Mathf.Cos(Time.time * floatSpeed) * floatAmount;

        
        transform.localRotation = Quaternion.Euler(0f, 0f, sway);
        transform.localPosition = startPosition + new Vector3(0f, floatOffset, 0f);
    }
}