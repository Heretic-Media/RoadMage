using System;
using UnityEngine;

public class Follow_player : MonoBehaviour
{
    [Tooltip("The angle at which the camera looks at the player")]
    [SerializeField] private float cameraAngle = 0f;

    [Tooltip("Multiplies the amount that the camera zooms out by due to velocity.")]
    [SerializeField] private float zoomMultiplier = 0.5f;

    [Tooltip("Minimum Camera height above the player")]
    [SerializeField] private float minimumCameraHeight = 40f;

    [Tooltip("The maximum speed at which the camera moves")]
    [SerializeField] private float cameraFollowSpeed = 30f;

    [Tooltip("This multiplies how much velocity should move the camera ahead of the player")]
    [SerializeField] private float lookAheadMultipler = 0.5f;

    [Tooltip("The most the camera should look ahead of the player")]
    [SerializeField] private float lookAheadMaximum = 50f;

    [Tooltip("Used when smoothing slow constant movement. Higher values reduce the amount of smoothing. Lower values limit the camera's speed")]
    [Range(0f, 1f)]
    [SerializeField] private float cameraSpeedSmoothingMultiplier = 0.21f;

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
        Vector3 horizontalFollow = (lastPlayerPosition + lookAhead - focusPosition).normalized * Mathf.Clamp(cameraFollowSpeed * Time.fixedDeltaTime, 0, (lastPlayerPosition + lookAhead - focusPosition).magnitude * cameraSpeedSmoothingMultiplier);
        horizontalFollow.y = 0;

        // here we apply horizontal offset
        focusPosition += horizontalFollow;

        // where the camera needs to get to vertically using its horizontal position to fill out the Vector3

        Vector3 cameraOffset = Quaternion.AngleAxis(cameraAngle, -player.right) * player.up * calculateZoom(playersVelocity.magnitude, minimumCameraHeight);

        Vector3 desiredCameraPos = focusPosition + cameraOffset;

        Quaternion desiredCameraRot = Quaternion.Euler(90, player.rotation.eulerAngles.y, 0) * Quaternion.Euler(-cameraAngle, 0, 0);

        //transform.eulerAngles = desiredCameraRot.eulerAngles;

        float lerpAmount = 4f;

        transform.eulerAngles = new Vector3(
Mathf.LerpAngle(transform.eulerAngles.x, desiredCameraRot.eulerAngles.x, Time.deltaTime * lerpAmount),
Mathf.LerpAngle(transform.eulerAngles.y, desiredCameraRot.eulerAngles.y, Time.deltaTime * lerpAmount),
Mathf.LerpAngle(transform.eulerAngles.z, desiredCameraRot.eulerAngles.z, Time.deltaTime * lerpAmount));

        // here we apply the movement from this update
        transform.position += (desiredCameraPos - transform.position);



        lastPlayerPosition = player.position;
    }
}