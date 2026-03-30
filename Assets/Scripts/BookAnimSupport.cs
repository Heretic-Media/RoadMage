using UnityEngine;

public class BookAnimSupport : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void launchAttack()
    {
        animator.SetBool("Book Attacking", false);
    }

    public void FixedUpdate()
    {
        launchAttack();
        this.enabled = false;
    }
}
