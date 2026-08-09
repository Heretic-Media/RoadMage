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

    private Animator animator;

    [SerializeField] private float dissolveSpeed = 0.5f;
    private MeshRenderer[] meshes;
    private bool destroyed = false;
    private float dissolveTimer = 0f;

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

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        meshes = GetComponentsInChildren<MeshRenderer>();
    }

    void MimicTriggered()
    {
        if (eventTriggered) return;
        eventTriggered = true;

        // pop up
        GameObject.FindGameObjectWithTag("EnemyPopUpManager").GetComponent<EnemyTutorialPopUps>().MimicPopUp();

        // play animation
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

        // pop up
        GameObject.FindGameObjectWithTag("EnemyPopUpManager").GetComponent<EnemyTutorialPopUps>().ChestPopUp();

        // play animation
        if (animator != null) animator.SetBool("chestOpening", true);

        // invoke a random chest event
        InvokeRandom(chestEvents);

        // destroy object after open animation
        Invoke(nameof(DestroyObject), 2.5f);
    }

    void DestroyObject()
    {
        destroyed = true;

        // stop animation
        //if (animator != null) animator.SetBool("mimicAttacking", false);
        //if (animator != null) animator.SetBool("chestOpening", false);
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
        /// Check destroyed
        if (destroyed) 
        {
            dissolveTimer += dissolveSpeed * Time.deltaTime;
        }

        foreach (var mesh in meshes)
        {
            mesh.material.SetFloat("_Progress", 0 + dissolveTimer);
        }

        if (dissolveTimer > 1) 
        {
            Destroy(gameObject);
        }
    }
}
