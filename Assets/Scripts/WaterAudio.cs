using UnityEngine;

public class WaterAudio : MonoBehaviour
{
    [SerializeField] GameObject waterAudioMAnager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterAudioMAnager.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                waterAudioMAnager.SetActive(false);
            }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterAudioMAnager.SetActive(true);
        }
    }
}
