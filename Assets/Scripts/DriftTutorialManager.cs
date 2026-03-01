using UnityEngine;

public class DriftTutorialManager : MonoBehaviour
{
    [SerializeField] float delay = 10f;

    // Update is called once per frame
    void FixedUpdate()
    {
        delay -= Time.fixedDeltaTime;

        if (delay < 0)
        {
            if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().ControllerConnected())
            {
                GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Hold <sprite=33> to drift.", 4f);
            }
            else if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().KeyboardConnected())
            {
                GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Hold <sprite=115> to drift.", 4f);
            }
            Destroy(gameObject);
        }
    }
}
