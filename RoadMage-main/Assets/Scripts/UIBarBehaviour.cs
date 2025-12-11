using UnityEngine;
using Unity.Mathematics;

public class UIBarBehaviour : MonoBehaviour
{
    [Tooltip("The object the bar will scale to indicate at the correct value.")]
    [SerializeField] private GameObject barFillObject;

    [Tooltip("The portion that the bar will be at initially.")]
    [SerializeField] private float initialValue = 0f;

    [SerializeField] private float lerpStrength = 0.5f;

    private Vector2 targetSize;

    private void Awake()
    {
        // first we find a needle if there is one and then rotate the needle to the starting position
        if (barFillObject == null)
        {
            if (transform.GetChild(0) == null)
            {
                Debug.LogError(gameObject.name + ": DialBehaviour has no needle object");
            }
            else
            {
                barFillObject = transform.GetChild(0).gameObject;
            }
        }

        targetSize = new Vector2(Mathf.Clamp(initialValue, 0f, 1f), 1f);
        barFillObject.GetComponent<RectTransform>().localScale = targetSize;
    }

    public void UpdateBar(float portionOfHealth)
    {
        // this takes a value between 0f and 1f and moves the needle to reflect a reading of that

        targetSize = new Vector2(Mathf.Clamp(portionOfHealth, 0f, 1f), 1f);
    }

    private void FixedUpdate()
    {
        barFillObject.GetComponent<RectTransform>().localScale = Vector2.Lerp(barFillObject.GetComponent<RectTransform>().localScale, targetSize, lerpStrength);
    }
}
