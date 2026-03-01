using UnityEngine;
using UnityEngine.Audio;

public class AudioSliders : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private AudioSource music;
    [SerializeField]
    private AudioSource sfx;

    public void ChangeMasterVolume(float value)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }

    public void ChangeMusicVolume(float value)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
    }

    public void ChangeSFXVolume(float value)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }
}
