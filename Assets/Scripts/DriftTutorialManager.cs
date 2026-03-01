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
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Hold <sprite=33> to drift.", 4f);
            Destroy(gameObject);
        }
    }
}
