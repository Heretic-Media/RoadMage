using UnityEngine;
using System.Collections;

public class PlayerRescue : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fallThreshold = -50f;
    [SerializeField] private float rescueHeight = 2f;
    [SerializeField] private Transform[] rescuePoints;

    [Header("NPC")]
    [SerializeField] private GameObject npcPrefab;

    private Rigidbody playerRb;
    private GameObject npc;
    private bool isRescuing = false;

    public static PlayerRescue Instance { get; private set; }

    public bool IsRescuing() => isRescuing;

    void Start()
    {
        Instance = this;
        playerRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isRescuing) return;

        if (transform.position.y < fallThreshold)
        {
            StartRescue();
        }
    }

    private Vector3 GetClosestRescuePosition()
    {
        if (rescuePoints == null || rescuePoints.Length == 0)
            return transform.position;

        Vector3 playerPos = transform.position;
        Transform closest = rescuePoints[0];
        float closestDistSqr = (closest.position - playerPos).sqrMagnitude;

        for (int i = 1; i < rescuePoints.Length; i++)
        {
            if (rescuePoints[i] == null) continue;
            float distSqr = (rescuePoints[i].position - playerPos).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = rescuePoints[i];
            }
        }

        return closest.position;
    }

    void StartRescue()
    {
        isRescuing = true;
        Vector3 rescuePosition = GetClosestRescuePosition();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        if (npcPrefab != null)
        {
            Vector3 spawnPos = transform.position;
            npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);
            npc.transform.rotation = Quaternion.LookRotation(rescuePosition + spawnPos);
            npc.name = "RescueNPC";

            RescueNPC npcScript = npc.GetComponent<RescueNPC>();
            if (npcScript != null)
            {
                npcScript.StartRescue(transform, rescuePosition, rescueHeight, OnRescueComplete);
            }
            else
            {
                SimpleRescue(rescuePosition);
            }
        }
        else
        {
            SimpleRescue(rescuePosition);
        }
    }

    void OnRescueComplete()
    {
        isRescuing = false;
        npc = null;
    }

    void SimpleRescue(Vector3 rescuePosition)
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
