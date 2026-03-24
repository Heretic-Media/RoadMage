using UnityEngine;

public class PortalColourShift : MonoBehaviour
{
    [SerializeField] private float shiftSpeed = 1f; // Speed of color shifting
    [SerializeField] private Color[] colors; // Array of colors to shift through
    private Color shiftColour;
    [SerializeField] private Material targetMaterial;
    [SerializeField] private GameObject light;

    // Update is called once per frame
    void Update()
    {
        float t = Time.time * shiftSpeed;
        int colorIndex = Mathf.FloorToInt(t) % colors.Length;
        int nextColorIndex = (colorIndex + 1) % colors.Length;
        float blend = t - Mathf.Floor(t);

        shiftColour = Color.Lerp(colors[colorIndex], colors[nextColorIndex], blend);
        targetMaterial.SetColor("_Base_Colour", shiftColour);
        targetMaterial.SetColor("_Emissive_Colour", shiftColour);

        if (light != null)
        {
            Light lightComponent = light.GetComponent<Light>();
            if (lightComponent != null)
            {
                lightComponent.color = shiftColour;
            }
        }
    }
}
