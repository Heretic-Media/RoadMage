using System;
using UnityEngine;
using UnityEngine.Events;

public class ChestController : MonoBehaviour
{
    private bool isMimic = false;
    private int mimicChance = 0;
    public UnityEvent mimicEvents;
    public UnityEvent[] chestEvents;

    private bool eventTriggered = false;

    void Awake()
    {
        mimicChance = UnityEngine.Random.Range(1, 10);
        if (mimicChance % 2 == 0)
        {
            // even = mimic time
            isMimic = true;
            gameObject.name = "MIMIC";
            gameObject.layer = 9; //enemy hurtbox
        }
        else
        {
            isMimic = false;
        }
    }

    void MimicTriggered()
    {
        if (eventTriggered) return;
        eventTriggered = true;

        // play animation
        var animator = gameObject.GetComponent<Animator>();
        if (animator != null) animator.SetBool("mimicAttacking", true);

        // invoke mimic event
        mimicEvents.Invoke();

        // destroy game object after attack animation
        Invoke(nameof(DestroyObject), 4.2f);
    }

    void ChestOpened()
    {
        if (eventTriggered) return;
        eventTriggered = true;

        // play animation
        var animator = gameObject.GetComponent<Animator>();
        if (animator != null) animator.SetBool("chestOpening", true);

        // invoke a random chest event
        InvokeRandom(chestEvents);

        // destroy object after open animation
        Invoke(nameof(DestroyObject), 2.5f);
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player detected");
            if (isMimic) MimicTriggered();
            else ChestOpened();
        }
    }

    private void InvokeRandom(UnityEvent[] events)
    {
        if (events == null || events.Length == 0) return;

        int index = UnityEngine.Random.Range(0, events.Length);
        var evt = events[index];
        if (evt != null) evt.Invoke();
    }

    void Update()
    {
    }
}
