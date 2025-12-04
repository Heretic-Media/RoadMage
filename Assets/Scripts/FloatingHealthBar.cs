using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    void Start()
    {
        
    }

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void UpdateHealthBar(float currentValue, float maxValue) 
    {
        slider.value = currentValue / maxValue;
    }
}
