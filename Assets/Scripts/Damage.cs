using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour
{
    // Script for handling simple attack data
    // Use the correct __Attack collision layer to avoid unintended interactions
    // Create hitbox as child of main gameObject
    // Trigger collider

    public int damage;
    // 0: Physical, 1: Fire, 2: Earth, 3: Water, 4: Air
    public int damageType;

    [SerializeField] int onHitEffect = 0;

    private void OnTriggerEnter(Collider other)
    {
        switch (onHitEffect)
        {
            default:
                break;
            case 1:
                DestroyParent();
                break;
        }
    }


    // onHitEffect functions
    public void DestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }
}
