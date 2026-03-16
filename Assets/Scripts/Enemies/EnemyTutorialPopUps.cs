using UnityEngine;

public class EnemyTutorialPopUps : MonoBehaviour
{
    private bool mimicPopUp = true;
    private bool chestPopUp = true;

    public void MimicPopUp()
    {
        if (mimicPopUp)
        {
            if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().ControllerConnected())
            {
                GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Use abilities when stunned by a mimic to defend or escape!", 6f);
            }
            else if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().KeyboardConnected())
            {
                GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Use abilities when stunned by a mimic to defend or escape!", 6f);
            }
            mimicPopUp=false;
        }
    }

    public void ChestPopUp()
    {
        if (chestPopUp)
        {
            if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().ControllerConnected())
            {
                GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Driving through chests triggers a random event!", 6f);
            }
            else if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().KeyboardConnected())
            {
                GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Driving through chests triggers a random event!", 6f);
            }
            chestPopUp=false;
        }
    }
}
