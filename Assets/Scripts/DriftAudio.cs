using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class DriftAudio : MonoBehaviour
{
    [SerializeField] AudioSource start;
    [SerializeField] AudioSource middle;
    [SerializeField] AudioSource end;


    [Tooltip("Time spent drifting for debugging")]
    [SerializeField] private float driftTime = 0;

    [Tooltip("Drift time needed before playing the sound effects")]
    [SerializeField] private float driftSoundDelay = 0.1f;


    private TopDownCarController carController;

    private bool playing = false;

    void Start()
    {
        carController = transform.parent.GetComponent<TopDownCarController>();
    }


    void Update()
    {
        transform.position = transform.parent.position;

        if (carController.drifting)
        {
            driftTime += Time.deltaTime;
        }
        else { driftTime = 0; }

        /// Drift Projectiles

        if (carController.drifting && Mathf.Abs(carController.rawSteerInput) > 0.5f && driftTime > driftSoundDelay && driftTime - driftSoundDelay >= 0)
        {
            // play audio
            if (!playing)
            {
                beginPlaying();
                playing = true;
            }
        }
        else
        {

            if (playing)
            {
                playing = false;
                endPlaying();
            }

        }
    }

    public void beginPlaying()
    {
        start.Play();
        Invoke("continuePlaying", start.clip.length);
    }

    public void endPlaying()
    {
        middle.Stop();
        start.Stop();

        end.Play();
    }

    private void continuePlaying()
    {
        if (playing)
        {
            middle.Play();
            middle.loop = true;
        }

    }
}
