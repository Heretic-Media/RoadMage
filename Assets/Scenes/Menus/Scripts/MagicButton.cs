using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
{
    public Image glow;
    Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;

        if (glow != null)
            glow.enabled = false;
    }

    void ActivateHover()
    {
        transform.localScale = baseScale * 1.1f;

        if (glow != null)
            glow.enabled = true;
    }

    void DeactivateHover()
    {
        transform.localScale = baseScale;

        if (glow != null)
            glow.enabled = false;
    }

    // Mouse hover
    public void OnPointerEnter(PointerEventData data)
    {
        ActivateHover();
    }

    public void OnPointerExit(PointerEventData data)
    {
        DeactivateHover();
    }

    // Controller / Keyboard selection
    public void OnSelect(BaseEventData eventData)
    {
        ActivateHover();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DeactivateHover();
    }
}