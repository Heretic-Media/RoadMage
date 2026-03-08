using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraBehaviour : MonoBehaviour
{
    private TopDownCarController playerController = null;
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

    [SerializeField] private bool relativeRotateCamera = false;

    [SerializeField] private float lerpMultiplier = 4f;

    [Header("Intro Animation")]
    [Tooltip("Enable the intro zoom animation on game start")]
    [SerializeField] private bool playIntroAnimation = true;

    [Tooltip("Starting distance above the car for intro (closer = smaller value)")]
    [SerializeField] private float introStartDistance = 10f;

    [Tooltip("Starting distance behind the car for intro")]
    [SerializeField] private float introStartOffsetZ = 5f;

    [Tooltip("How long the zoom animation takes in seconds")]
    [SerializeField] private float introAnimationDuration = 2f;

    [Tooltip("Animation curve for smooth zoom (set in Inspector for custom easing)")]
    [SerializeField] private AnimationCurve introAnimationCurve;

    private Vector3 shakeOffset = Vector3.zero;
    private Vector3 anchorPos;
    private Transform player;
    private Vector3 lastPlayerPosition;
    private Vector3 focusPosition;

    private bool introAnimationPlaying = false;

    void Start()
    {
        anchorPos = transform.position;
        transform.rotation = Quaternion.Euler(90, 0, -180) * Quaternion.Euler(-cameraAngle, 0, 0);

        if (player == null)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
            {
                Debug.LogWarning("Follow_player: player Transform is not assigned.");
                lastPlayerPosition = transform.position;
            }
            else
            {
                player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
                playerController = player.GetComponent<TopDownCarController>();
                lastPlayerPosition = player.position;
                focusPosition = lastPlayerPosition;
            }
        }
        else
        {
            lastPlayerPosition = player.position;
            playerController = player.GetComponent<TopDownCarController>();
        }

        if (playIntroAnimation && player != null)
        {
            StartCoroutine(PlayIntroAnimationCoroutine());
        }
        else
        {
            transform.position = player.position + new Vector3(0, minimumCameraHeight, 0);
        }
    }

    IEnumerator PlayIntroAnimationCoroutine()
    {
        introAnimationPlaying = true;
        float elapsedTime = 0f;

        Vector3 playerStartPos = player.position;

        Quaternion gameplayRotation;
        if (relativeRotateCamera)
        {
            gameplayRotation = Quaternion.Euler(90, player.rotation.eulerAngles.y, 0) * Quaternion.Euler(-cameraAngle, 0, 0);
        }
        else
        {
            gameplayRotation = Quaternion.Euler(90, 0, -180) * Quaternion.Euler(-cameraAngle, 0, 0);
        }

        transform.rotation = gameplayRotation;

        Vector3 offsetRight = gameplayRotation * Vector3.right;
        Vector3 startCameraOffset = Quaternion.AngleAxis(cameraAngle, -offsetRight) * Vector3.up * introStartDistance;
        Vector3 endCameraOffset = Quaternion.AngleAxis(cameraAngle, -offsetRight) * Vector3.up * minimumCameraHeight;

        Vector3 startPos = playerStartPos + startCameraOffset;
        Vector3 endPos = playerStartPos + endCameraOffset;

        transform.position = startPos;
        anchorPos = startPos;
        lastPlayerPosition = playerStartPos;
        focusPosition = playerStartPos;

        if (playerController != null)
        {
            playerController.disabledTime = float.MaxValue;
        }

        while (elapsedTime < introAnimationDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / introAnimationDuration);
            float curveValue = introAnimationCurve != null ? introAnimationCurve.Evaluate(progress) : progress;

            Vector3 currentPos = Vector3.Lerp(startPos, endPos, curveValue);
            transform.position = currentPos;
            anchorPos = currentPos;

            if (relativeRotateCamera)
            {
                Quaternion desiredCameraRot = Quaternion.Euler(90, player.rotation.eulerAngles.y, 0) * Quaternion.Euler(-cameraAngle, 0, 0);
                transform.rotation = desiredCameraRot;
            }
            else
            {
                transform.rotation = gameplayRotation;
            }

            yield return null;
        }

        transform.position = endPos;
        anchorPos = endPos;
        focusPosition = playerStartPos;
        transform.rotation = gameplayRotation;

        if (playerController != null)
        {
            playerController.disabledTime = 0f;
        }

        introAnimationPlaying = false;
    }

    float calculateZoom(float currentVelocity, float minOffset = 100f)
    {
        return minOffset + currentVelocity * zoomMultiplier;
    }

    void FixedUpdate()
    {
        if (introAnimationPlaying) return;

        if (player == null)
            throw new InvalidOperationException("Follow_player: player Transform is not assigned.");

        Vector3 playersVelocity = player.GetComponent<Rigidbody>().linearVelocity;

        Vector3 lookAhead = playersVelocity * lookAheadMultipler;
        if (lookAhead.magnitude > lookAheadMaximum)
        {
            lookAhead = lookAhead.normalized * lookAheadMaximum;
        }

        Vector3 horizontalFollow = (lastPlayerPosition + lookAhead - focusPosition).normalized * Mathf.Clamp(cameraFollowSpeed * Time.fixedDeltaTime, 0, (lastPlayerPosition + lookAhead - focusPosition).magnitude * cameraSpeedSmoothingMultiplier);
        horizontalFollow.y = 0;

        focusPosition += horizontalFollow;

        if (relativeRotateCamera)
        {
            Quaternion desiredCameraRot = Quaternion.Euler(90, player.rotation.eulerAngles.y, 0) * Quaternion.Euler(-cameraAngle, 0, 0);

            transform.eulerAngles = new Vector3(
    Mathf.LerpAngle(transform.eulerAngles.x, desiredCameraRot.eulerAngles.x, Time.deltaTime * lerpMultiplier),
    Mathf.LerpAngle(transform.eulerAngles.y, desiredCameraRot.eulerAngles.y, Time.deltaTime * lerpMultiplier),
    Mathf.LerpAngle(transform.eulerAngles.z, desiredCameraRot.eulerAngles.z, Time.deltaTime * lerpMultiplier));
        }
        else
        {
            transform.rotation = Quaternion.Euler(90, 0, -180) * Quaternion.Euler(-cameraAngle, 0, 0);
        }

        Vector3 cameraOffset = Quaternion.AngleAxis(cameraAngle, -transform.right) * Vector3.up * calculateZoom(playersVelocity.magnitude, minimumCameraHeight);

        Vector3 desiredCameraPos = focusPosition + cameraOffset;

        anchorPos += (desiredCameraPos - anchorPos);

        transform.position = anchorPos + shakeOffset;

        lastPlayerPosition = player.position;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraBehaviour>().ShakeCoRoutine(duration, magnitude));
    }

    IEnumerator ShakeCoRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            shakeOffset += new Vector3(UnityEngine.Random.Range(-1f, 1), 0, UnityEngine.Random.Range(-1f, 1)) * magnitude;

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }

    public bool IsIntroPlaying()
    {
        return introAnimationPlaying;
    }

    public void PlayIntroAnimation()
    {
        if (player == null) return;

        StopAllCoroutines();
        StartCoroutine(PlayIntroAnimationCoroutine());
    }

    public void SkipIntroAnimation()
    {
        StopAllCoroutines();

        Vector3 targetPos = player.position + new Vector3(0, minimumCameraHeight, 0);
        transform.position = targetPos;
        anchorPos = targetPos;
        focusPosition = player.position;

        transform.rotation = Quaternion.Euler(90, 0, -180) * Quaternion.Euler(-cameraAngle, 0, 0);

        if (playerController != null)
        {
            playerController.disabledTime = 0f;
        }

        introAnimationPlaying = false;

        Debug.Log("Intro animation skipped");
    }
}