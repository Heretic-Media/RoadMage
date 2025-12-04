using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image glow;
    Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
        if (glow != null) glow.enabled = false;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        transform.localScale = baseScale * 1.1f;
        if (glow) glow.enabled = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        transform.localScale = baseScale;
        if (glow) glow.enabled = false;
    }
}
