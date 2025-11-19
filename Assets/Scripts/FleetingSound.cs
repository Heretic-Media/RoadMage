using UnityEngine;

public class FleetingSound : MonoBehaviour
{
    private AudioSource m_AudioSource;

    private float waitTime;

    private float timer = 0f;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        waitTime = m_AudioSource.clip.length;
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer > waitTime)
        {
            Destroy(gameObject);
        }
    }
}
