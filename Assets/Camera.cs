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

    [Tooltip("Multiplies the amount that the camera zooms out by due to velocity.")]
    [SerializeField] private float zoomMultiplier = 0.5f;

    [Tooltip("Minimum Camera height above the player")]
    [SerializeField] private float minimumCameraHeight = 40f;

    [Tooltip("The speed at which the camera follows the player")]
    [SerializeField] private float cameraFollowSpeed = 100f;

    [Tooltip("This multiplies how much velocity should move the camera ahead of the player")]
    [SerializeField] private float lookAheadMultipler = 0.5f;

    [Tooltip("The most the camera should look ahead of the player")]
    [SerializeField] private float lookAheadMaximum = 50f;

    [Tooltip("The angle at which the camera looks at the player")]
    [SerializeField] private float cameraAngle = 0f;

    private Transform player; // as long as the player is tagged we can find them in Start()
    private Vector3 lastPlayerPosition;
    private Vector3 focusPosition; // focus position is what the camera is looking at

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
                lastPlayerPosition = player.position;
                focusPosition = lastPlayerPosition;
            }

        }
        else
        {
            lastPlayerPosition = player.position;
            // Not keen on this, but it prevents a jarring snap on start. 
            transform.position = player.position + new Vector3(0, minimumCameraHeight, 0);
        }
    }

    float calculateZoom(float currentVelocity, float minOffset = 100f)
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
        Vector3 horizontalFollow = (lastPlayerPosition + lookAhead - focusPosition).normalized * Mathf.Clamp(cameraFollowSpeed * Time.fixedDeltaTime, 0, (lastPlayerPosition + lookAhead - focusPosition).magnitude);
        horizontalFollow.y *= 0;

        // here we apply horizontal offset
        focusPosition += horizontalFollow;

        // where the camera needs to get to vertically using its horizontal position to fill out the Vector3

        Vector3 cameraOffset = Quaternion.AngleAxis(cameraAngle, Vector3.right) * Vector3.up * calculateZoom(playersVelocity.magnitude, minimumCameraHeight);

        Vector3 desiredCameraPos = focusPosition + cameraOffset;

        transform.rotation = Quaternion.Euler(90, 0, -180) * Quaternion.Euler(-cameraAngle, 0, 0);

        // here we apply vertical offset
        transform.position += (desiredCameraPos - transform.position).normalized * Mathf.Clamp(cameraFollowSpeed * Time.fixedDeltaTime, 0, (desiredCameraPos - transform.position).magnitude);

        lastPlayerPosition = player.position;
    }
}