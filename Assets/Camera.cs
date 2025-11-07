using System;
using UnityEngine;

public class Follow_player : MonoBehaviour
{
    //[Tooltip("Base smooth time used when player is stationary")]
    //[SerializeField] private float baseSmoothTime = 0.25f;

    //[Tooltip("How much the smoothTime increases per unit of player speed")]
    //[SerializeField] private float speedToSmoothTime = 0.02f;

    //[Tooltip("Minimum smooth time clamp")]
    //[SerializeField] private float minSmoothTime = 0.02f;

    //[Tooltip("Maximum smooth time clamp")]
    //[SerializeField] private float maxSmoothTime = 1.0f;

    [Tooltip("Minimum Camera height above the player")]
    [SerializeField] private float minimumCameraHeight = 40f;

    [Tooltip("The speed at which the camera follows the player")]
    [SerializeField] private float cameraFollowSpeed = 100f;

    [Tooltip("This multiplies how much velocity should move the camera ahead of the player")]
    [SerializeField] private float lookAheadMultipler = 1f;

    [Tooltip("The most the camera should look ahead of the player")]
    [SerializeField] private float lookAheadMaximum = 50f;

    private Transform player; // as long as the player is tagged we can find them in Start()
    private Vector3 lastPlayerPosition;


    void Start()
    {
        if (player == null)
        {
            // if there are multiple player objects this needs re-writing
            if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
            {
                Debug.LogWarning("Follow_player: player Transform is not assigned.");
                lastPlayerPosition = transform.position;
            }
            else
            {
                player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
            }

        }
        else
        {
            lastPlayerPosition = player.position;
            // Not keen on this, but it prevents a jarring snap on start. 
            transform.position = player.position + new Vector3(0, minimumCameraHeight, 0);
        }
    }

    float calculateZoom(float currentVelocity, float minOffset = 100f, float zoomMultiplier = 1.5f)
    {
        return minOffset + currentVelocity * zoomMultiplier;
    }

    void FixedUpdate()
    {
        if (player == null) // Mainly checking for myself. Also useful if something goes horribly wrong for the sake of debugging.
            throw new InvalidOperationException("Follow_player: player Transform is not assigned.");

        Vector3 playersVelocity = player.GetComponent<Rigidbody>().linearVelocity;

        Vector3 lookAhead = playersVelocity * lookAheadMultipler;
        if (lookAhead.magnitude > lookAheadMaximum)
        {
            lookAhead = lookAhead.normalized * lookAheadMaximum;
        }

        // where the camera needs to get to but the y component is 0
        Vector3 horizontalFollow = (lastPlayerPosition + lookAhead - transform.position).normalized * Mathf.Clamp(cameraFollowSpeed * Time.fixedDeltaTime, 0, (lastPlayerPosition - transform.position).magnitude);
        horizontalFollow.y *= 0;

        // here we apply horizontal offset
        transform.position += horizontalFollow;

        // where the camera needs to get to vertically using its horizontal position to fill out the Vector3
        Vector3 desiredCameraPos = new Vector3(transform.position.x, calculateZoom(playersVelocity.magnitude, minimumCameraHeight), transform.position.z);

        // here we apply vertical offset
        transform.position += (desiredCameraPos - transform.position).normalized * Mathf.Clamp(cameraFollowSpeed * Time.fixedDeltaTime, 0, (desiredCameraPos - transform.position).magnitude);

        lastPlayerPosition = player.position;
    }
}