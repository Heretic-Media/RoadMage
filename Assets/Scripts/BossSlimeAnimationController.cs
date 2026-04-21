using UnityEngine;

public class BossSlimeAnimationController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("attacking", true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("attacking", false);
            }
        }
    }
}
