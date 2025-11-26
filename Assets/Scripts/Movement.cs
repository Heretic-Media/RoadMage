using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class TopDownCarController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    [Header("Axis Locking")]
    public bool lockAxis = false;
    [Tooltip("Set to 0 to disable")]
    public float uprightTorque = 250;
    [Tooltip("Angle in degrees player must tilt to start correcting to upright")]
    public float degreeThreshold = 15;
    [Tooltip("Debugging to show when upright tilt is active")]
    public bool tryingToUpright = false;

    [Header("Speed (m/s)")]
    public float maxForwardSpeed = 20f;
    public float maxReverseSpeed = 10f;

    [Header("Acceleration / Braking")]
    public float acceleration = 25f;
    public float reverseAcceleration = 30f;
    public float brakeStrength = 55f;
    public float idleDrag = 0.75f;
    public float accelDrag = 0.6f;
    public float brakeDrag = 5f;
    [Tooltip("Debugging to show when breaking")]
    public bool isBraking = false;
    public float brakeFactor = 6f;

    [Header("Steering")]
    public float maxSteerAnglePerSec = 280f;
    [Range(0.1f, 1f)]
    public float steerAtTopSpeedFactor = 0.6f;
    public float steeringResponse = 10f;

    [Header("Grip / Traction")]
    [Tooltip("Base sideways grip. Higher = less slide.")]
    public float baseLateralGrip = 20f;
    [Tooltip("Extra grip when accelerating.")]
    public float accelLateralGripBoost = 4f;
    [Tooltip("Extra grip when braking.")]
    public float brakeLateralGripBoost = 6f;
    [Tooltip("Handbrake grip factor when NOT using drift mode.")]
    public float handbrakeLateralGripFactor = 0.35f;

    [Header("Drift")]
    public bool enableDrift = true;
    [Tooltip("Lateral grip multiplier while drifting.")]
    public float driftLateralGripFactor = 1f;
    [Tooltip("Forward drag while drifting (keeps momentum, slight slowdown).")]
    public float driftForwardDrag = 0.2f;
    [Tooltip("Steering multiplier while drifting.")]
    public float driftSteerMultiplier = 0.4f;
    [Tooltip("Extra yaw torque while drifting to help kick the rear out.")]
    public float driftYawBoost = 6f;
    [Tooltip("Prevents extra yaw torque from building up")]
    public float maxYawVelocity = 3f;
    [Tooltip("Minimum speed needed before drift takes effect.")]
    public float driftMinSpeed = 5f;

    public bool enableDriftProjectiles = true;
    public float driftTime = 0;
    public float driftProjectileDelay = 1f;
    public float driftProjectileRate = 0.5f;
    private float timeSinceLastDriftProjectile = 0;

    [Header("Input / Physics")]
    public Key handbrakeKey = Key.LeftCtrl;
    public Key driftKey = Key.Space;
    public Rigidbody rb;
    public Vector3 centerOfMassOffset = new Vector3(0f, -0.4f, 0f);

    [Header("World-Relative Settings")]
    [Tooltip("The world direction that 'forward' input moves toward")]
    public Vector3 worldForward = Vector3.forward;
    [Tooltip("The world direction that 'right' input moves toward")]
    public Vector3 worldRight = Vector3.right;

    float rawThrottleInput;
    float rawSteerInput;
    float steerInputSmoothed;
    bool handbrake;
    bool drift;

    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        if (lockAxis)
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.centerOfMass += centerOfMassOffset;
        }
        else
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            rb.centerOfMass += centerOfMassOffset;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
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
            (gp != null && gp.buttonSouth.isPressed) ||
            (kb != null && kb[driftKey].isPressed);
    }

    void FixedUpdate()
    {
        if (!rb) return;

        // Keep rb upright
        if (Vector3.Dot(transform.up, Vector3.up) < Mathf.Cos(degreeThreshold * Mathf.Deg2Rad))
        {
            tryingToUpright = true;
            Vector3 torqueAxis = Vector3.Cross(transform.up, Vector3.up);
            rb.AddTorque(torqueAxis * uprightTorque * Time.fixedDeltaTime);
        }
        else
        {
            tryingToUpright = false;
        }


        Vector3 inputDirection = (worldForward * rawThrottleInput + worldRight * rawSteerInput).normalized;
        float inputMagnitude = new Vector2(rawThrottleInput, rawSteerInput).magnitude;
        inputMagnitude = Mathf.Clamp01(inputMagnitude);


        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        float speedAbs = Mathf.Abs(forwardSpeed);

        if (audioSource != null)
            audioSource.volume = speedAbs / 20f;

        bool hasInput = inputMagnitude > 0.05f;
        bool drifting = enableDrift && drift && speedAbs > driftMinSpeed;

        if (drifting)
        {
            driftTime += Time.fixedDeltaTime;
        }
        else
        {
            driftTime = 0;
        }


        float targetAngle = transform.eulerAngles.y;
        if (hasInput)
        {
            targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        }

        float currentAngle = transform.eulerAngles.y;
        float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);

        Vector3 velocityDir = rb.linearVelocity.normalized;
        bool movingBackward = hasInput && Vector3.Dot(inputDirection, velocityDir) < -0.5f && speedAbs > 0.5f;

        isBraking = movingBackward;

      
        if (hasInput && !handbrake)
        {
            if (!isBraking)
            {
                if (forwardSpeed < maxForwardSpeed)
                {
                    float accelForce = acceleration * inputMagnitude;
                    rb.AddForce(transform.forward * accelForce, ForceMode.Acceleration);
                }
            }
            else
            {
                rb.AddForce(-velocityDir * brakeStrength, ForceMode.Acceleration);
            }
        }
        else if (handbrake)
        {
            Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
            localVel.z = Mathf.MoveTowards(localVel.z, 0f, brakeFactor * Time.fixedDeltaTime);
            rb.linearVelocity = transform.TransformDirection(localVel);
        }

        // Drag
        float targetDrag;
        if (drifting)
        {
            targetDrag = driftForwardDrag;
        }
        else if (!hasInput)
        {
            targetDrag = idleDrag;
        }
        else if (isBraking)
        {
            targetDrag = brakeDrag;
        }
        else
        {
            targetDrag = accelDrag;
        }

        rb.linearDamping = Mathf.Lerp(rb.linearDamping, targetDrag, Time.fixedDeltaTime * 8f);

        // Lateral grip
        float lateralGrip = baseLateralGrip;
        if (hasInput) lateralGrip += accelLateralGripBoost;
        if (isBraking) lateralGrip += brakeLateralGripBoost;

        if (drifting)
        {
            lateralGrip *= driftLateralGripFactor;
        }
        else if (handbrake)
        {
            lateralGrip *= handbrakeLateralGripFactor;
        }

        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        float lateral = localVelocity.x;
        float lateralKill = lateralGrip * Time.fixedDeltaTime;
        localVelocity.x = Mathf.MoveTowards(lateral, 0f, lateralKill);

        Vector3 newWorldVel = transform.TransformDirection(localVelocity);
        newWorldVel.y = rb.linearVelocity.y;
        rb.linearVelocity = newWorldVel;

     
        localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        localVelocity.z = Mathf.Clamp(localVelocity.z, -maxReverseSpeed, maxForwardSpeed);
        rb.linearVelocity = transform.TransformDirection(localVelocity);

        if (hasInput && !isBraking)
        {
            float minSteerSpeed = 1.5f;
            float steerFactorBySpeed = Mathf.InverseLerp(minSteerSpeed, maxForwardSpeed, speedAbs);
            steerFactorBySpeed = Mathf.Clamp01(steerFactorBySpeed);

            float highSpeedSteerScale = Mathf.Lerp(1f, steerAtTopSpeedFactor, steerFactorBySpeed);

            steerInputSmoothed = Mathf.MoveTowards(
                steerInputSmoothed,
                Mathf.Sign(angleDiff),
                steeringResponse * Time.fixedDeltaTime
            );

            float steerThisFrame = steerInputSmoothed * maxSteerAnglePerSec * highSpeedSteerScale * steerFactorBySpeed;

            if (drifting)
            {
                steerThisFrame *= driftSteerMultiplier;

                if (Mathf.Abs(rb.angularVelocity.y) < maxYawVelocity)
                {
                    rb.AddTorque(Vector3.up * Mathf.Sign(angleDiff) * driftYawBoost, ForceMode.Acceleration);
                }
            }

            float maxRotThisFrame = Mathf.Abs(steerThisFrame) * Time.fixedDeltaTime;
            float actualRotation = Mathf.Clamp(angleDiff, -maxRotThisFrame, maxRotThisFrame);

            if (Mathf.Abs(actualRotation) > 0.01f)
            {
                Quaternion deltaRot = Quaternion.Euler(0f, actualRotation, 0f);
                rb.MoveRotation(rb.rotation * deltaRot);
            }
        }

        // Drift Projectiles
        if (drifting && inputMagnitude > 0.5f && enableDriftProjectiles)
        {
            timeSinceLastDriftProjectile += Time.fixedDeltaTime;
            if (timeSinceLastDriftProjectile >= driftProjectileRate)
            {
                timeSinceLastDriftProjectile = 0;

                print("spawning drift projectile");
                if (driftTime > driftProjectileDelay)
                {
                    SpawnProjectile(rb.linearVelocity.magnitude * 0.5f);
                }
                else
                {
                    SpawnProjectile(driftTime / driftProjectileDelay * rb.linearVelocity.magnitude * 0.5f);
                }
            }
        }
    }

    private void SpawnProjectile(float projectileSpeed)
    {
        if (projectilePrefab == null)
            return;

        Vector3 spawnPos = transform.position - transform.forward * 0.6f + Vector3.up * 0.2f;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Vector3 dir = -transform.forward;
        Rigidbody projRb = proj.GetComponent<Rigidbody>();
        if (projRb == null)
        {
            projRb = proj.AddComponent<Rigidbody>();
            projRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRb.interpolation = RigidbodyInterpolation.Interpolate;
            projRb.useGravity = false;
        }
        projRb.linearVelocity = dir * projectileSpeed;
    }
}