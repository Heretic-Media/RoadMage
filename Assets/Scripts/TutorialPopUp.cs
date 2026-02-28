using TMPro;
using UnityEngine;

public class TutorialPopUp : MonoBehaviour
{
    private float timer = 9999f;
    private Vector2 targetPos = Vector2.zero;

    public void SetText(string value)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = value;
        GetComponentInChildren<RectTransform>().anchoredPosition = Vector3.zero;
    }

    public void SetTimer(float value)
    {
        timer = value;
    }

    public void SetPositionAuto(int childrenCount)
    {
        targetPos = new Vector2(200 + (200 * (childrenCount - 1)), -135);
    }

    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;

        if (timer < 0)
        {
            GetComponent<RectTransform>().parent.GetComponent<TutorialPopUpManager>().OrderReorder();
            Destroy(gameObject);
        }

        if (targetPos != GetComponent<RectTransform>().anchoredPosition)
        {
            GetComponent<RectTransform>().anchoredPosition += 0.1f * (targetPos - GetComponent<RectTransform>().anchoredPosition); 
        }
    }
}
