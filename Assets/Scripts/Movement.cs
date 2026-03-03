using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Rigidbody))]
public class TopDownCarController : MonoBehaviour
{
    [Header("Axis Locking")]
    [SerializeField] private bool lockAxis = false;
    [Tooltip("Set to 0 to disable")]
    [SerializeField] private float uprightTorque = 250;
    [Tooltip("Angle in degrees player must tilt to start correcting to upright")]
    [SerializeField] private float degreeThreshold = 15;
    [Tooltip("Debugging to show when upright tilt is active")]
    [SerializeField] private bool tryingToUpright = false;

    [Header("Speed (m/s)")]
    [SerializeField] private float maxForwardSpeed = 20f;
    [SerializeField] private float maxReverseSpeed = 10f;

    [Header("Acceleration / Braking")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float reverseAcceleration = 30f;
    [SerializeField] private float brakeStrength = 55f;
    [SerializeField] private float idleDrag = 0.75f;
    [SerializeField] private float accelDrag = 0.6f;
    [SerializeField] private float brakeDrag = 5f;
    [Tooltip("Debugging to show when breaking")]
    [SerializeField] private bool isBraking = false;
    [SerializeField] private float brakeFactor = 6f;

    // Increase maxSteerAnglePerSec to reduce turning circle
    [Header("Steering")]
    [SerializeField] private float minSteerThreshold = 0.3f;
    [SerializeField] private float maxSteerAnglePerSec = 280f; // was 140f, increased for tighter turns
    [Range(0.1f, 1f)]
    [SerializeField] private float steerAtTopSpeedFactor = 0.6f;
    [SerializeField] private float steeringResponse = 10f;

    [Header("Grip / Traction")]
    [Tooltip("Base sideways grip. Higher = less slide.")]
    [SerializeField] private float baseLateralGrip = 20f;
    [Tooltip("Extra grip when accelerating.")]
    [SerializeField] private float accelLateralGripBoost = 4f;
    [Tooltip("Extra grip when braking.")]
    [SerializeField] private float brakeLateralGripBoost = 6f;
    [Tooltip("Handbrake grip factor when NOT using drift mode.")]
    [SerializeField] private float handbrakeLateralGripFactor = 0.35f;

    [Header("Drift")]
    [SerializeField] private bool enableDrift = true;
    [Tooltip("Lateral grip multiplier while drifting.")]
    [SerializeField] private float driftLateralGripFactor = 1f;
    [Tooltip("Forward drag while drifting (keeps momentum, slight slowdown).")]
    [SerializeField] private float driftForwardDrag = 0.2f;
    [Tooltip("Steering multiplier while drifting.")]
    [SerializeField] private float driftSteerMultiplier = 0.4f;
    [Tooltip("Extra yaw torque while drifting to help kick the rear out.")]
    [SerializeField] private float driftYawBoost = 6f;
    [Tooltip("Prevents extra yaw torque from building up")]
    [SerializeField] private float maxYawVelocity = 3f;
    [Tooltip("Minimum speed needed before drift takes effect.")]
    [SerializeField] private float driftMinSpeed = 5f;

    [Header("Input / Physics")]
    [SerializeField] private Key handbrakeKey = Key.LeftCtrl;
    [SerializeField] private Key driftKey = Key.Space;
    public Rigidbody rb;
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0f, -0.4f, 0f);

    [SerializeField] private float rawThrottleInput;
    public float rawSteerInput;
    private float steerInputSmoothed;
    private bool handbrake;
    private bool drift;

    [SerializeField] private bool hasThrottle;
    public bool drifting;

    public float disabledTime = 2f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject rightSparks;
    [SerializeField] private GameObject leftSparks;
    [SerializeField] private Damage rightSparksHitbox;
    [SerializeField] private Damage leftSparksHitbox;

    private GameObject speedGauge;

    // These are all used for tutorial pop-up management
    private float tutorialTimeDriven = 0f;
    private float tutorialTimeDriftingBraking = 0f;
    private int tutorialStage = 0;

    // made this a toggle for testing the block out level feel free to switch it back - Cy
    void Awake()
    {
        if (lockAxis)
        {
            if (!rb) rb = GetComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.centerOfMass += centerOfMassOffset;
        }

        else if (!lockAxis)
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            rb.centerOfMass += centerOfMassOffset;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        //find speed gauge
        speedGauge = GameObject.FindGameObjectWithTag("SpeedFuelGauge");

        //tutorial setup
        if (GameObject.FindGameObjectWithTag("TutorialMovement") != null)
        {
            GameObject.FindGameObjectWithTag("TutorialMovement").GetComponent<Canvas>().enabled = true;
        }
    }

    void Update()
    {
        if (disabledTime > 0)
        {
            disabledTime -= Time.deltaTime;
            if (disabledTime < 0) disabledTime = 0;
        }
        else
        {
            var kb = Keyboard.current;
            var gp = Gamepad.current;

            float t = 0f;

            if (gp != null)
            {
                t = gp.rightTrigger.ReadValue() - gp.leftTrigger.ReadValue();
            }
            else if (kb != null)
            {
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) t += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) t -= 1f;
            }

            rawThrottleInput = Mathf.Clamp(t, -1f, 1f);

            float s = 0f;

            if (gp != null)
            {
                s = gp.leftStick.x.ReadValue();
            }
            else if (kb != null)
            {
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) s -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) s += 1f;
            }

            rawSteerInput = Mathf.Clamp(s, -1f, 1f);

            handbrake =
                (gp != null && gp.leftShoulder.isPressed) ||
                (kb != null && kb[handbrakeKey].isPressed);

            drift =
                (gp != null && gp.buttonSouth.isPressed) || // A button on most controllers
                (kb != null && kb[driftKey].isPressed);

            // speed gauge
            if (speedGauge != null)
            {
                speedGauge.GetComponent<UIDialBehaviour>().UpdateGauge(rb.linearVelocity.magnitude / maxForwardSpeed / 2);
            }

            // tutorial management
            if (tutorialStage == 0)
            {
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed || kb.sKey.isPressed || kb.downArrowKey.isPressed)
                {
                    tutorialTimeDriven += Time.deltaTime;
                    if (tutorialTimeDriven > 5f)
                    {
                        if (GameObject.FindGameObjectWithTag("TutorialDriftingBraking") != null)
                        {
                            GameObject.FindGameObjectWithTag("TutorialDriftingBraking").GetComponent<Canvas>().enabled = true;
                        }
                        tutorialStage = 1;
                    }
                }
            }
            else if (tutorialStage == 1)
            {
                if ((gp != null && gp.leftShoulder.isPressed) || (kb != null && kb[handbrakeKey].isPressed) || (gp != null && gp.buttonSouth.isPressed) || (kb != null && kb[driftKey].isPressed))
                {
                    tutorialTimeDriftingBraking += Time.deltaTime;
                    if (tutorialTimeDriftingBraking > 5f)
                    {
                        if (GameObject.FindGameObjectWithTag("TutorialDriftingBraking") != null)
                        {
                            GameObject.FindGameObjectWithTag("TutorialDriftingBraking").GetComponent<Canvas>().enabled = false;
                        }
                        if (GameObject.FindGameObjectWithTag("TutorialMovement") != null)
                        {
                            GameObject.FindGameObjectWithTag("TutorialMovement").GetComponent<Canvas>().enabled = false;
                        }
                        tutorialStage = 2;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (disabledTime > 0) return;
        if (!rb) return;

        /// Keep rb upright
        if (Vector3.Dot(transform.up, Vector3.up) < Mathf.Cos(degreeThreshold * Mathf.Deg2Rad))
        {
            tryingToUpright = true;

            // Calculate torque axis using cross product
            Vector3 torqueAxis = Vector3.Cross(transform.up, Vector3.up);
            rb.AddTorque(torqueAxis * uprightTorque * Time.fixedDeltaTime);
        }
        else
        {
            tryingToUpright = false;
        }

        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        float forwardSpeed = localVel.z;
        float speedAbs = Mathf.Abs(forwardSpeed);

        audioSource.volume = speedAbs / 20;

        hasThrottle = Mathf.Abs(rawThrottleInput) > 0.05f;

        drifting = enableDrift && drift && speedAbs > driftMinSpeed && Mathf.Abs(rawSteerInput) > minSteerThreshold;

        isBraking =
            (forwardSpeed > 0.2f && rawThrottleInput < -0.1f) ||
            (forwardSpeed < -0.2f && rawThrottleInput > 0.1f);


        if (hasThrottle && !handbrake)
        {
            Vector3 lockedAxisForward = new Vector3(transform.forward.x, Vector3.forward.y, transform.forward.z);
            if (!isBraking)
            {
                if (rawThrottleInput > 0f)
                {
                    if (forwardSpeed < maxForwardSpeed)
                        rb.AddForce(lockedAxisForward * (rawThrottleInput * acceleration), ForceMode.Acceleration);
                }
                else
                {
                    if (forwardSpeed > -maxReverseSpeed)
                        rb.AddForce(lockedAxisForward * (rawThrottleInput * reverseAcceleration), ForceMode.Acceleration);
                }
            }
            else
            {
                rb.AddForce(-Mathf.Sign(forwardSpeed) * lockedAxisForward * brakeStrength, ForceMode.Acceleration);
            }
        }
        else if (handbrake)
        {
            localVel.z = Mathf.MoveTowards(localVel.z, 0f, brakeFactor * Time.fixedDeltaTime);
        }

        /// Drag
        float targetDrag;

        if (drifting)
        {
            targetDrag = driftForwardDrag;
        }
        else
        {
            if (!hasThrottle)
                targetDrag = idleDrag;
            else if (isBraking)
                targetDrag = brakeDrag;
            else
                targetDrag = accelDrag;
        }

        rb.linearDamping = Mathf.Lerp(rb.linearDamping, targetDrag, Time.fixedDeltaTime * 8f);

        /// Turning
        float lateralGrip = baseLateralGrip;

        if (hasThrottle) lateralGrip += accelLateralGripBoost;
        if (isBraking) lateralGrip += brakeLateralGripBoost;

        if (drifting)
        {
            lateralGrip *= driftLateralGripFactor;
        }
        else if (handbrake && !drifting)
        {
            lateralGrip *= handbrakeLateralGripFactor;
        }

        float lateral = localVel.x;
        float lateralKill = lateralGrip * Time.fixedDeltaTime;
        localVel.x = Mathf.MoveTowards(lateral, 0f, lateralKill);

        // Don't set velocity if kinematic (e.g., during rescue)
        if (rb.isKinematic) return;

        Vector3 newWorldVel = transform.TransformDirection(localVel);
        newWorldVel.y = rb.linearVelocity.y;
        rb.linearVelocity = newWorldVel;

        localVel = transform.InverseTransformDirection(rb.linearVelocity);
        forwardSpeed = Mathf.Clamp(localVel.z, -maxReverseSpeed, maxForwardSpeed);
        localVel.z = forwardSpeed;
        rb.linearVelocity = transform.TransformDirection(localVel);

        float minSteerSpeed = 1.5f;
        float steerFactorBySpeed = Mathf.InverseLerp(minSteerSpeed, maxForwardSpeed, Mathf.Abs(forwardSpeed));
        steerFactorBySpeed = Mathf.Clamp01(steerFactorBySpeed);

        float highSpeedSteerScale = Mathf.Lerp(1f, steerAtTopSpeedFactor, steerFactorBySpeed);

        steerInputSmoothed = Mathf.MoveTowards(
            steerInputSmoothed,
            rawSteerInput,
            steeringResponse * Time.fixedDeltaTime
        );

        float steerThisFrame =
            steerInputSmoothed *
            maxSteerAnglePerSec *
            highSpeedSteerScale *
            steerFactorBySpeed;

        if (forwardSpeed < -0.2f)
            steerThisFrame *= -1f;

        if (drifting)
        {
            steerThisFrame *= driftSteerMultiplier;

            if (Mathf.Abs(rb.angularVelocity.y) < maxYawVelocity)
            {
                rb.AddTorque(Vector3.up * steerInputSmoothed * driftYawBoost, ForceMode.Acceleration);
            }
        }

        if (Mathf.Abs(steerThisFrame) > 0.01f)
        {
            Quaternion deltaRot = Quaternion.Euler(0f, steerThisFrame * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * deltaRot);
        }

        /// Drift Particles
        if (drifting && Mathf.Abs(rawSteerInput) > 0.5f && rawThrottleInput > 0f)
        {
            if (rawSteerInput > 0.5f && steerThisFrame > 30f)
            {
                rightSparks.GetComponent<ParticleSystem>().Stop();
                leftSparks.GetComponent<ParticleSystem>().Play();
                rightSparksHitbox.gameObject.SetActive(false);
                leftSparksHitbox.gameObject.SetActive(true);
            }
            else if (rawSteerInput < -0.5f && steerThisFrame < 30f)
            {
                rightSparks.GetComponent<ParticleSystem>().Play();
                leftSparks.GetComponent<ParticleSystem>().Stop();
                rightSparksHitbox.gameObject.SetActive(true);
                leftSparksHitbox.gameObject.SetActive(false);
            }

        }
        else
        {
            rightSparks.GetComponent<ParticleSystem>().Stop();
            leftSparks.GetComponent<ParticleSystem>().Stop();
            rightSparksHitbox.gameObject.SetActive(false);
            leftSparksHitbox.gameObject.SetActive(false);
        }
    }
}