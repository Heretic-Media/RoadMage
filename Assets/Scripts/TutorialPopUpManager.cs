using UnityEngine;

public class TutorialPopUpManager : MonoBehaviour
{
    [SerializeField] GameObject tutorialPopupPrefab;

    private bool reorderNeeded = false;

    public void OrderReorder()
    {
        reorderNeeded = true;
    }

    public void StartTutorialPopup(string textToUse, float duration)
    {
        GameObject spawned = Instantiate(tutorialPopupPrefab);

        spawned.GetComponent<TutorialPopUp>().SetText(textToUse);
        spawned.GetComponent<TutorialPopUp>().SetTimer(duration);
        spawned.transform.SetParent(transform);

        PositionEachChild();
    }

    public void PositionEachChild()
    {
        int num = 1;
        
        foreach (TutorialPopUp child in GetComponentsInChildren<TutorialPopUp>())
        {
            child.SetPositionAuto(num);
            num++;
        }
    }

    private void Awake()
    {
        StartTutorialPopup("RT to accelerate", 6f);
        StartTutorialPopup("LT to reverse", 6f);
    }

    private void LateUpdate()
    {
        if (reorderNeeded)
        {
            PositionEachChild();
            reorderNeeded = false;
        }
    }
}
