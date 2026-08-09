using UnityEngine;

public class DisableIconsforQuarterSecond : MonoBehaviour
{
    private float starting_time;
    [SerializeField] private float end_time = 0.25f;
    private void Awake()
    {
        starting_time = Time.time;
    }

    private void Update()
    {
        if (starting_time < Time.time - end_time)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
