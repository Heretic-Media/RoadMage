using UnityEngine;

public class DisableforQuarterSecond : MonoBehaviour
{
    private float starting_time;
    
    private void Awake()
    {
        starting_time = Time.time;
    }

    private void Update()
    {
        if (starting_time < Time.time - 0.25f)
        {
            GetComponent<AudioSource>().volume = 0.5f;
        }
    }
}
