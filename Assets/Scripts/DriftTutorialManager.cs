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
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("A to drift.", 4f);
            Destroy(gameObject);
        }
    }
}
