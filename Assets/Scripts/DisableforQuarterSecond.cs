using UnityEngine;

public class DisableforQuarterSecond : MonoBehaviour
{
    private float starting_time;
    [SerializeField] private float end_time = 0.25f;
    [SerializeField] private float end_volume = 0.5f;
    private void Awake()
    {
        starting_time = Time.time;
    }

    private void Update()
    {
        if (starting_time < Time.time - end_time)
        {
            GetComponent<AudioSource>().volume = end_volume;
        }
    }
}
