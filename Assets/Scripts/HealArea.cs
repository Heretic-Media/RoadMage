using System.Collections.Generic;
using UnityEngine;

public class HealArea : MonoBehaviour
{
    [SerializeField] private int healAmount = 100;
    [SerializeField] private float healAfterTime = 5;
    [SerializeField] private float timeElapsed = 0;

    private HashSet<Collider> colliders = new HashSet<Collider>();
    public HashSet<Collider> GetColliders() { return colliders; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeElapsed > healAfterTime) 
        {
            colliders.RemoveWhere(c => c == null);
            foreach (Collider collider in colliders)
            {
                if (collider != null && collider.enabled) 
                {
                    Health colliderHealth = collider.gameObject.GetComponent<Health>();
                    if (colliderHealth != null)
                    {
                        colliderHealth.TakeDamage(-healAmount);
                    }
                }
                else
                {
                    colliders.Remove(collider);
                }
            }
            Destroy(this.gameObject);

            timeElapsed = 0;
        }
        timeElapsed += Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        colliders.Add(other); //hashset automatically handles duplicates
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }
}
