using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class DriftAudio : MonoBehaviour
{
    [SerializeField] AudioSource start;
    [SerializeField] AudioSource middle;
    [SerializeField] AudioSource end;

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
        middle.Play();
        middle.loop = true;
    }
}
