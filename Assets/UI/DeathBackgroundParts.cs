using UnityEngine;
using UnityEngine.UI;

public class MagicParticle : MonoBehaviour
{
    [Header("Fall Settings")]
    public float fallSpeed = 20f;
    public float swayAmount = 10f;
    public float swaySpeed = 2f;

    [Header("Fade")]
    public float fadeSpeed = 1.5f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 0.8f;

    [Header("Bounds")]
    public float resetHeight = 600f;  
    public float bottomLimit = -600f; 

    private RectTransform rect;
    private Image img;

    private float swayOffset;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();

        swayOffset = Random.Range(0f, 10f);

        float scale = Random.Range(0.4f, 1.2f);
        transform.localScale = Vector3.one * scale;

        
        fallSpeed *= Random.Range(0.7f, 1.3f);

        if (img != null)
        {
            Color c = img.color;
            c.b += Random.Range(0f, 0.2f); 
            img.color = c;
        }
    }

    void Update()
    {
       
        rect.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

        float sway = Mathf.Sin((Time.time + swayOffset) * swaySpeed) * swayAmount;
        rect.anchoredPosition += new Vector2(sway * Time.deltaTime, 0);

     
        if (rect.anchoredPosition.y < bottomLimit)
        {
            float randomX = Random.Range(-800f, 800f);
            rect.anchoredPosition = new Vector2(randomX, resetHeight);
        }

        
        if (img != null)
        {
            float t = (Mathf.Sin((Time.time + swayOffset) * fadeSpeed) + 1f) / 2f;

            Color c = img.color;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
            img.color = c;
        }
    }
}