using UnityEngine;
using System.Collections;

public class PlayerRescue : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fallThreshold = -50f;
    [SerializeField] private float rescueHeight = 2f;
    [SerializeField] private Transform spawnPoint;

    [Header("NPC")]
    [SerializeField] private GameObject npcPrefab;

    private Rigidbody playerRb;
    private Vector3 rescuePosition;
    private GameObject npc;
    private bool isRescuing = false;

    public static PlayerRescue Instance { get; private set; }

    public bool IsRescuing() => isRescuing;

    void Start()
    {
        Instance = this;
        playerRb = GetComponent<Rigidbody>();

        if (spawnPoint == null)
        {
            rescuePosition = transform.position;
        }
        else
        {
            rescuePosition = spawnPoint.position;
        }
    }

    void Update()
    {
        if (isRescuing) return;

        if (transform.position.y < fallThreshold)
        {
            StartRescue();
        }
    }

    void StartRescue()
    {
        isRescuing = true;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        if (npcPrefab != null)
        {
            Vector3 spawnPos = transform.position;
            npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);
            npc.name = "RescueNPC";

            RescueNPC npcScript = npc.GetComponent<RescueNPC>();
            if (npcScript != null)
            {
                npcScript.StartRescue(transform, rescuePosition, rescueHeight, OnRescueComplete);
            }
            else
            {
                SimpleRescue();
            }
        }
        else
        {
            SimpleRescue();
        }
    }

    void OnRescueComplete()
    {
        isRescuing = false;
        npc = null;
    }

    void SimpleRescue()
    {
        transform.position = new Vector3(rescuePosition.x, rescueHeight, rescuePosition.z);
        OnRescueComplete();
    }

    void OnDestroy()
    {
        if (npc != null)
        {
            Destroy(npc);
        }
    }
}