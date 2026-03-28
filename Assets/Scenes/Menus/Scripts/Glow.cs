using UnityEngine;
using UnityEngine.UI;

public class StarTwinkle : MonoBehaviour
{
    public float speed = 2f;

    public float minBrightness = 0.7f;
    public float maxBrightness = 1.2f;

    private Image img;
    private float offset;

    void Start()
    {
        img = GetComponent<Image>();
        offset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float pulse = (Mathf.Sin((Time.time + offset) * speed) + 1f) / 2f;

        float brightness = Mathf.Lerp(minBrightness, maxBrightness, pulse);

        if (img != null)
        {
            img.color = new Color(brightness, brightness, brightness, 1f);
        }
    }
}