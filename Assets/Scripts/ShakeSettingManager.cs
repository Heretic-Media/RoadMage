using UnityEngine;

public class ShakeSettingManager : MonoBehaviour
{
    [Range(0.0f, 4.0f)]
    [SerializeField] public float shakeStrength = 1.0f;
}
