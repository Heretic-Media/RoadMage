using UnityEngine;

public class PopUpOnWeapon : MonoBehaviour
{
    [SerializeField]
    private string contents;
    [SerializeField] private float duration;

    private void awake()
    {
        GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup(contents, duration);
    }

}
