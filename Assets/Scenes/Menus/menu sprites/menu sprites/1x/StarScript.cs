using UnityEngine;
using UnityEngine.UI;

public class StarScript : MonoBehaviour
{
    public float speed = 2f;        
    public float minAlpha = 0.4f;   // Dimmest point
    public float maxAlpha = 1f;     // Brightest point

    private Image img;
    private float randomOffset;

    void Start()
    {
        img = GetComponent<Image>();

        
        randomOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float t = (Mathf.Sin((Time.time + randomOffset) * speed) + 1f) / 2f;

        Color c = img.color;
        c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        img.color = c;
    }
}