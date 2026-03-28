using UnityEngine;

public class GeneralColourShift : MonoBehaviour
{
    [SerializeField] private bool changeConstantly = true; // Toggle for constant color shifting
    [SerializeField] private bool synced = true;
    [SerializeField] private bool randomlyAssignChildMats = false;
    private bool isShifted = false; // Flag to track if color shifting is active
    [SerializeField] private float shiftSpeed = 1f; // Speed of color shifting
    [SerializeField] private Color[] colors; // Array of colors to shift through
    private Color shiftColour;
    [SerializeField] private Material[] targetMaterials;

    private void Start()
    {
        if (!randomlyAssignChildMats || targetMaterials == null || targetMaterials.Length == 0)
        {
            return;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
        {
            r.sharedMaterial = targetMaterials[Random.Range(0, targetMaterials.Length)];
        }
    }

    private void ApplyColourToMaterials(Color colour)
    {
        if (targetMaterials == null || targetMaterials.Length == 0)
        {
            return;
        }

        if (synced)
        {
            for (int i = 0; i < targetMaterials.Length; i++)
            {
                targetMaterials[i].color = colour;
                targetMaterials[i].SetColor("_EmissionColor", colour);
            }

            return;
        }

        int colourCount = colors != null ? colors.Length : 0;
        if (colourCount == 0)
        {
            return;
        }

        int[] availableIndices = new int[colourCount];
        for (int i = 0; i < colourCount; i++)
        {
            availableIndices[i] = i;
        }

        for (int i = 0; i < targetMaterials.Length; i++)
        {
            int randomIndex = Random.Range(0, colourCount);
            int colourIndex = availableIndices[randomIndex];

            availableIndices[randomIndex] = availableIndices[colourCount - 1];
            colourCount = Mathf.Max(0, colourCount - 1);

            Color randomColour = colors[colourIndex];
            targetMaterials[i].color = randomColour;
            targetMaterials[i].SetColor("_EmissionColor", randomColour);

            if (colourCount == 0)
            {
                colourCount = colors.Length;
                for (int j = 0; j < colourCount; j++)
                {
                    availableIndices[j] = j;
                }
            }
        }
    }

    private void ColourShift()
    {
        if (changeConstantly)
        {
            float t = Time.time * shiftSpeed;
            int colorIndex = Mathf.FloorToInt(t) % colors.Length;
            int nextColorIndex = (colorIndex + 1) % colors.Length;
            float blend = t - Mathf.Floor(t);

            shiftColour = Color.Lerp(colors[colorIndex], colors[nextColorIndex], blend);
            ApplyColourToMaterials(shiftColour);
        }

        else if (!changeConstantly && !isShifted)
        {
            float t = Time.time * shiftSpeed;
            int colorIndex = Mathf.FloorToInt(t) % colors.Length;
            int nextColorIndex = (colorIndex + 1) % colors.Length;
            float blend = t - Mathf.Floor(t);

            shiftColour = Color.Lerp(colors[colorIndex], colors[nextColorIndex], blend);
            ApplyColourToMaterials(shiftColour);
            isShifted = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ColourShift();
    }
}
