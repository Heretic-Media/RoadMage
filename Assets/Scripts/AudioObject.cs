using UnityEngine;

public class AudioObject : MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
