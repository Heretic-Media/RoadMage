using UnityEngine;

public class ButtonBurstTrigger : MonoBehaviour
{
    public ParticleSystem burst;
    public RectTransform buttonRect;

    public void PlayBurst()
    {
        burst.transform.position = buttonRect.position;
        burst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        burst.Play();
       // burst.transform.position = buttonRect.position + new Vector3(0, -20, 0);

    }
}
