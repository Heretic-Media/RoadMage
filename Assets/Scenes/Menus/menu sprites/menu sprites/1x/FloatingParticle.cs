using UnityEngine;

public class FloatingParticle : MonoBehaviour
{
    public float speed = 10f;
    public float floatAmount = 20f;

    private Vector3 startPos;
    private float randomOffset;

    void Start()
    {
        startPos = transform.localPosition;
        randomOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float x = Mathf.Sin((Time.time + randomOffset) * speed) * floatAmount;
        float y = Mathf.Cos((Time.time + randomOffset) * speed) * floatAmount;

        transform.localPosition = startPos + new Vector3(x, y, 0f);
    }
}