using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Brightness : MonoBehaviour
{
    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        var volume = gameObject.GetComponent<Volume>();
        if (volume == null || volume.profile == null)
        {
            Debug.LogWarning("[Brightness] Volume or VolumeProfile is missing.");
            return;
        }

        if (!volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.LogWarning("[Brightness] VolumeProfile does not contain a ColorAdjustments override.");
            colorAdjustments = null;
        }
    }

    public void AdjustBrightness(float exposureEV)
    {
        if (colorAdjustments == null) return;
        colorAdjustments.postExposure.Override(exposureEV);
    }
}
