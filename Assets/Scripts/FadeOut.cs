using UnityEngine;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private float starting_volume = 0.7f;
    [SerializeField] private float duration = 6f;

    private void Awake()
    {
        GetComponent<AudioSource>().volume = starting_volume;
    }

    private void FixedUpdate()
    {
        GetComponent<AudioSource>().volume = Mathf.Max(GetComponent<AudioSource>().volume - Time.fixedDeltaTime / duration, 0);
    }
}
