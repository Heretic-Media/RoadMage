using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Speeds (m/s)")]
    public float maxForwardSpeed = 20f;       // forward top speed
    public float maxReverseSpeed = 6f;        // reverse top speed (slower than forward)

    [Header("Longitudinal Forces")]
    public float forwardAcceleration = 30f;   // forward acceleration (ForceMode.Acceleration)
    public float reverseAcceleration = 12f;   // reverse acceleration (weaker -> feels slower)
    public float brakeForce = 80f;            // strong braking when opposite throttle applied
    public float engineBrakingDamping = 1f;   // linear damping applied when no throttle
    public float handbrakeDamping = 6f;       // stronger damping with handbrake

    [Header("Steering")]
    [Tooltip("Max steering rotation in degrees/sec at low speed")]
    public float maxSteerDegPerSec = 160f;
    [Range(0.1f, 1f), Tooltip("Factor applied to steering at top speed (smaller = less steering at high speed)")]
    public float steerAtTopSpeedFactor = 0.35f;

    [Header("Lateral / Traction")]
    [Range(0f, 20f)]
    public float lateralGrip = 8f;            // how quickly sideways velocity is removed (higher = less sliding)

    [Header("References / Tuning")]
    public Rigidbody rb;
    public Vector3 centerOfMassOffset = Vector3.zero; // useful to lower CoM for stability

    [Header("Braking Multipliers")]
    public float brakeDampingMultiplier = 4f; // Linear braking, not handbrake 
    public float handBrakeDampingMultiplier = 10f; // Stronger damping when handbrake is applied
    

    // runtime inputs
    float steerInput;
    float throttleInput;
    bool handbrake;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Keep roll/pitch locked, yaw free. Set center of mass for stability.
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.centerOfMass += centerOfMassOffset;
        }
    }

    void Update()
    {
        // Read inputs using the new Input System (keyboard or gamepad)
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        // Throttle: W/up = forward, S/down = reverse. Gamepad left stick Y is prioritized when present.
        throttleInput = 0f;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) throttleInput = 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) throttleInput = -1f;
        }
        if (gp != null)
        {
            throttleInput = gp.leftStick.y.ReadValue();
        }


        // Steering: A/D or gamepad left stick X — only when moving forward
        steerInput = 0f;
        float localForward = 0f;
        float localBackward = 0f;
        if (rb != null)
        {
            Vector3 localVelForCheck = transform.InverseTransformDirection(rb.linearVelocity);

            localForward = localVelForCheck.z;
            localBackward = -localVelForCheck.z;
        }

        // Only allow steering when moving forward above threshold (1 m/s)
        if (localForward > 1f || localBackward > 1f)
        {
            if (kb != null)
            {
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) steerInput = -1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) steerInput = 1f;
            }
            if (gp != null)
            {
                steerInput = gp.leftStick.x.ReadValue();
            }
        }
        else
        {
            // not moving forward: ensure no steering input
            steerInput = 0f;
        }

        // Handbrake: Space or gamepad south button
        handbrake = (kb != null && kb.spaceKey.isPressed) || (gp != null && gp.buttonSouth.isPressed);
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Convert velocity into local space for easier logic
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        float forwardSpeed = localVel.z; // forward component (m/s), can be negative for reversing

        // --- Longitudinal behaviour (accel / reverse / brake) ---
        if (throttleInput > 0.01f)
        {
            // accelerating forward
            if (forwardSpeed < maxForwardSpeed)
                rb.AddForce(transform.forward * throttleInput * forwardAcceleration, ForceMode.Acceleration);
        }
        else if (throttleInput < -0.01f)
        {
            // reversing or braking
            // if currently moving forward and throttle is reverse, apply stronger brakeForce first
            if (forwardSpeed > 0.5f)
            {
                rb.AddForce(-transform.forward * Mathf.Abs(throttleInput) * brakeForce, ForceMode.Acceleration);
            }
            else
            {
                // apply reverse acceleration (weaker than forward)
                if (forwardSpeed > -maxReverseSpeed)
                    rb.AddForce(transform.forward * throttleInput * reverseAcceleration, ForceMode.Acceleration); // throttleInput is negative
            }
        }
        else
        {
            // no throttle: engine braking via linear damping
            rb.linearDamping = Mathf.Lerp(rb.linearDamping, engineBrakingDamping, Time.fixedDeltaTime * brakeDampingMultiplier);
        }

        // Handbrake increases damping for quick stops / slides
        if (handbrake)
        {
            rb.linearDamping = Mathf.Lerp(rb.linearDamping, handbrakeDamping, Time.fixedDeltaTime * handBrakeDampingMultiplier);

        }
        else
        {
            // reduce damping while accelerating
            if (Mathf.Abs(throttleInput) > 0.01f)
                rb.linearDamping = Mathf.Lerp(rb.linearDamping, 0f, Time.fixedDeltaTime * 8f);
        }

        // --- Lateral grip: reduce sideways velocity for traction ---
        // localVel.x is the lateral velocity (m/s). Damp it toward zero to simulate tire grip.
        float newLateral = Mathf.Lerp(localVel.x, 0f, lateralGrip * Time.fixedDeltaTime);
        localVel.x = newLateral;

        // Keep vertical (y) world velocity component unchanged
        Vector3 newWorldVel = transform.TransformDirection(new Vector3(localVel.x, 0f, localVel.z));
        newWorldVel.y = rb.linearVelocity.y;
        rb.linearVelocity = newWorldVel;

        // --- Speed capping: forward and reverse separately ---
        // Recompute local forward speed after potential changes
        localVel = transform.InverseTransformDirection(rb.linearVelocity);
        forwardSpeed = localVel.z;

        if (forwardSpeed > maxForwardSpeed)
        {
            localVel.z = maxForwardSpeed;
            Vector3 capped = transform.TransformDirection(localVel);
            capped.y = rb.linearVelocity.y;
            rb.linearVelocity = capped;
        }
        else if (forwardSpeed < -maxReverseSpeed)
        {
            localVel.z = -maxReverseSpeed;
            Vector3 capped = transform.TransformDirection(localVel);
            capped.y = rb.linearVelocity.y;
            rb.linearVelocity = capped;
        }

        // --- Steering: steering decreases with speed; inverted when reversing ---
        float speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / Mathf.Max(0.01f, maxForwardSpeed));
        float steerScale = Mathf.Lerp(1f, steerAtTopSpeedFactor, speedFactor); // reduce steering at high speed
        float appliedSteer = steerInput * maxSteerDegPerSec * steerScale;

        // Invert steering when reversing for realistic feel
        if (forwardSpeed < -0.1f) appliedSteer = -appliedSteer;

        if (Mathf.Abs(appliedSteer) > 0.01f)
        {
            Quaternion deltaRot = Quaternion.Euler(0f, appliedSteer * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * deltaRot);
        }
    }
}