using UnityEngine;
using System.Collections;

public class RescueNPC : MonoBehaviour
{
    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    [Header("Speed")]
    [SerializeField] private float liftSpeed = 10f;
    [SerializeField] private float flySpeed = 15f;
    [SerializeField] private float dropSpeed = 5f;

    [Header("Altitude")]
    [Tooltip("Height to fly to during rescue")]
    [SerializeField] private float flightAltitude = 15f;

    private Transform player;
    private Vector3 destination;
    private float dropHeight;
    private Rigidbody playerRb;
    private System.Action onComplete;

    public void StartRescue(Transform playerTransform, Vector3 dest, float height, System.Action callback)
    {
        player = playerTransform;
        destination = dest;
        dropHeight = height;
        onComplete = callback;

        if (player != null)
        {
            Vector3 playerWorldPos = player.position;
            player.SetParent(transform);
            player.position = playerWorldPos;
            player.GetComponent<Player>().ToggleTrails(false);

            playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
                playerRb.isKinematic = true;
            }

            StartCoroutine(RescueCoroutine());
        }
    }

    IEnumerator RescueCoroutine()
    {
        yield return StartCoroutine(Phase1_Lift());

        yield return StartCoroutine(Phase2_Travel());

        yield return StartCoroutine(Phase3_Drop());

        onComplete?.Invoke();

        Destroy(gameObject);
    }

    IEnumerator Phase1_Lift()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, flightAltitude, startPos.z);

        while (transform.position.y < flightAltitude - 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, liftSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward * 0.7f + Vector3.up * 0.3f), Time.deltaTime * 2f);
            yield return null;
        }
    }

    IEnumerator Phase2_Travel()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(destination.x, startPos.y, destination.z);

        while (Vector3.Distance(transform.position, targetPos) > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, flySpeed * Time.deltaTime);
            Vector3 direction = (targetPos - transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 2f);
            yield return null;
        }
    }

    IEnumerator Phase3_Drop()
    {
        Vector3 npcFixedPosition = transform.position;

        if (player != null && player.parent == transform)
        {
            player.SetParent(null);
            player.GetComponent<Player>().ToggleTrails(true);
        }

        if (playerRb != null)
        {
            playerRb.isKinematic = false;
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        yield return new WaitForSeconds(1f);

        Vector3 flyAwayEnd = transform.position + transform.forward * 10f;
        float flyAwayTime = 0f;
        float flyAwayDuration = 3f;

        while (flyAwayTime < flyAwayDuration)
        {
            flyAwayTime += Time.deltaTime;
            flyAwayTime = Mathf.Min(flyAwayTime, flyAwayDuration);
            float t = flyAwayTime / flyAwayDuration;
            transform.position = Vector3.Lerp(transform.position, flyAwayEnd, t);
            yield return null;
        }

    }
}