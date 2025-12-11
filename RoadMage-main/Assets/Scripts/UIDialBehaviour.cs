using Unity.Mathematics;
using UnityEngine;

public class UIDialBehaviour : MonoBehaviour
{
    [Tooltip("The object the dial will use to point at the correct value.")]
    [SerializeField] private GameObject needleObject;

    [Tooltip("The maximum rotation of the needle in either direction in degrees.")]
    [SerializeField] private float bounds = 70f;

    [Tooltip("The center of the of the dial in degrees.")]
    [SerializeField] private float rotationalOffset = 0f;

    [Tooltip("The portion that the gauge will be at initially.")]
    [SerializeField] private float initialValue = 0f;

    [SerializeField] private float lerpStrength = 0.5f;

    private quaternion targetRot;

    private void Awake()
    {
        // first we find a needle if there is one and then rotate the needle to the starting position
        if (needleObject == null)
        {
            if (transform.GetChild(0) == null)
            {
                Debug.LogError(gameObject.name + ": DialBehaviour has no needle object");
            }
            else
            {
                needleObject = transform.GetChild(0).gameObject;
            }
        }

        targetRot = Quaternion.Euler(0f, 0f, rotationalOffset + bounds - (Mathf.Clamp(initialValue, 0f, 1f) * bounds * 2));
        needleObject.GetComponent<RectTransform>().rotation = targetRot;
    }

    public void UpdateGauge(float portionOfHealth)
    {
        // this takes a value between 0f and 1f and moves the needle to reflect a reading of that
        
        targetRot = Quaternion.Euler(0f, 0f, rotationalOffset + bounds - (Mathf.Clamp(portionOfHealth, 0f, 1f) * bounds * 2));
    }

    private void FixedUpdate()
    {
        needleObject.GetComponent<RectTransform>().rotation = Quaternion.Lerp(needleObject.GetComponent<RectTransform>().rotation, targetRot, lerpStrength);
    }
}
