using UnityEngine;

public class MagicalFloat : MonoBehaviour
{
    public float amplitude = 10f;
    public float frequency = 1f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}
