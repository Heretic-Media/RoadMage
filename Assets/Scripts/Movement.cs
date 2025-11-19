using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

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

    // Increase maxSteerAnglePerSec to reduce turning circle
    [Header("Steering")]
    public float maxSteerAnglePerSec = 280f; // was 140f, increased for tighter turns
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

    float rawThrottleInput;
    float rawSteerInput;
    float steerInputSmoothed;
    bool handbrake;
    bool drift;

    [SerializeField] private AudioSource audioSource;
  

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
            (gp != null && gp.buttonSouth.isPressed) || // A button on most controllers
            (kb != null && kb[driftKey].isPressed);
    }

    void FixedUpdate()
    {
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

        bool hasThrottle = Mathf.Abs(rawThrottleInput) > 0.05f;

        bool drifting = enableDrift && drift && speedAbs > driftMinSpeed;

        if (drifting)
        {
            driftTime += 1 * Time.deltaTime;
        }
        else { driftTime = 0; }

        isBraking =
            (forwardSpeed > 0.2f && rawThrottleInput < -0.1f) ||
            (forwardSpeed < -0.2f && rawThrottleInput > 0.1f);


        if (hasThrottle && !handbrake)
        {
            if (!isBraking)
            {
                if (rawThrottleInput > 0f)
                {
                    if (forwardSpeed < maxForwardSpeed)
                        rb.AddForce(transform.forward * (rawThrottleInput * acceleration), ForceMode.Acceleration);
                }
                else
                {
                    if (forwardSpeed > -maxReverseSpeed)
                        rb.AddForce(transform.forward * (rawThrottleInput * reverseAcceleration), ForceMode.Acceleration);
                }
            }
            else
            {
                rb.AddForce(-Mathf.Sign(forwardSpeed) * transform.forward * brakeStrength, ForceMode.Acceleration);
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

        /// Drift Projectiles

        if (drifting && Mathf.Abs(rawSteerInput) > 0.5f && enableDriftProjectiles) 
        {
            timeSinceLastDriftProjectile += Time.deltaTime;
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
                    SpawnProjectile(driftTime/driftProjectileDelay * rb.linearVelocity.magnitude * 0.5f);
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