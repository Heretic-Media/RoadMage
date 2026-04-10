using System.Collections.Generic;
using UnityEngine;

public class DriftAbility : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject fireAudio;

    [SerializeField] private int audioPlayersCount = 5;
    private List<GameObject> fireSounds = new List<GameObject>();
    private int audioIndex = 0;

    

    [Tooltip("Toggle the ability on or off")]
    public bool enableDriftProjectiles = true;

    [Tooltip("Time spent drifting for debugging")]
    [SerializeField] private float driftTime = 0;
    public float driftDelayTime = 0;

    [Tooltip("Drift time needed before spawning first projectile")]
    [SerializeField] private float driftAbilityDelay = 1f;
    [Tooltip("Drift time needed before projectile speed starts decaying")]
    [SerializeField] private float driftAbilityDecay = 4f;
    [Tooltip("Drift time needed before projectile stop spawning")]
    [SerializeField] private float driftAbilityBurnout = 10f;
    [Tooltip("Time multiplier for ability cooldown")]
    [SerializeField] private float driftAbilityCooldownRate = 2f;



    [Tooltip("The fire rate of the projectiles")]
    [SerializeField] private float driftProjectileRate = 0.05f;
    [Tooltip("The random spawn direction range")]
    [SerializeField] private float driftProjectileRandomness = 0.3f;

    private float timeSinceLastDriftProjectile = 0;
    private bool fireEffectTriggered = false;

    private TopDownCarController carController;
    private ScreenEffects screenEffects;


    void Start()
    {
        carController = transform.parent.GetComponent<TopDownCarController>();
        screenEffects = GameObject.FindGameObjectWithTag("ScreenEffects").GetComponent<ScreenEffects>();

        for (int i = 0; i < audioPlayersCount; i++) 
        {
            fireSounds.Add(Instantiate(fireAudio, Vector3.zero, Quaternion.identity));
        }
    }

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().ControllerConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Hold <sprite=33> to shoot flames while drifting.", 6f);
        }
        else if (GameObject.FindGameObjectWithTag("IntuitiveSwitching").GetComponent<UIChangeInputIcons>().KeyboardConnected())
        {
            GameObject.FindGameObjectWithTag("TutorialPopUpManager").GetComponent<TutorialPopUpManager>().StartTutorialPopup("Hold <sprite=115> to shoot flames while drifting.", 6f);
        }
    }

    void Update()
    {
        transform.position = transform.parent.position;
        
        if (carController.drifting)
        {
            if (driftDelayTime < driftAbilityDelay) 
            {
                driftDelayTime += Time.deltaTime;
            }
            else 
            {
                if (driftTime >= driftAbilityBurnout)
                {
                    driftTime = driftAbilityBurnout;
                }
                else
                {
                    driftTime += Time.deltaTime;
                }
            }
        }
        else 
        {
            driftDelayTime = 0;

            if (driftTime <= 0) 
            {
                driftTime = 0;
            }
            else 
            {
                driftTime -= driftAbilityCooldownRate * Time.deltaTime;
            } 
        }

        /// Drift Projectiles

        if (carController.drifting && Mathf.Abs(carController.rawSteerInput) > 0.5f && enableDriftProjectiles && driftDelayTime > driftAbilityDelay)
        {
            timeSinceLastDriftProjectile += Time.deltaTime;

            // Debug.Log("spawning drift projectile");
            if (driftTime >= driftAbilityBurnout)
            {
                // Debug.Log("drift ability timed out");
                StopFireEffectIfNeeded();
            }
            else if (driftTime <= driftAbilityDecay)
            {
                if (timeSinceLastDriftProjectile >= driftProjectileRate)
                {
                    SpawnProjectile(carController.rb.linearVelocity.magnitude * 0.5f);
                }
            }
            else
            {
                float burnoutTime = (driftAbilityBurnout - driftAbilityDecay);
                float burnoutDifference = burnoutTime - (driftTime - driftAbilityDecay);

                if (timeSinceLastDriftProjectile <= driftProjectileRate * 2 + (1 - burnoutDifference / burnoutTime)) { }
                else if (burnoutDifference > 0)
                {
                    SpawnProjectile(burnoutDifference / burnoutTime
                        * carController.rb.linearVelocity.magnitude * 0.5f);
                }
            }
        }
        else
        {
            StopFireEffectIfNeeded();
        }
    }

    private void StopFireEffectIfNeeded()
    {
        if (!fireEffectTriggered)
        {
            return;
        }

        if (screenEffects != null)
        {
            screenEffects.StopFireEffect();
        }

        fireEffectTriggered = false;
    }

    private void StartAudio()
    {
        if (fireSounds[audioIndex].GetComponentInChildren<AudioSource>().isPlaying == false) 
        {
            fireSounds[audioIndex].GetComponentInChildren<AudioSource>().Play();
        }
        else 
        {
            audioIndex++;
            audioIndex = audioIndex % audioPlayersCount;
            fireSounds[audioIndex].GetComponentInChildren<AudioSource>().Stop();
            StartAudio();
        }
    }

    private void SpawnProjectile(float projectileSpeed)
    {
        timeSinceLastDriftProjectile = 0;

        if (projectile == null)
            return;

        if (!fireEffectTriggered && screenEffects != null)
        {
            screenEffects.TriggerFireEffect();
            fireEffectTriggered = true;
        }

        StartAudio();

        Vector3 spawnPos = transform.position - transform.forward * 0.6f + Vector3.up * 0.2f;
        GameObject proj = Instantiate(projectile, spawnPos, Quaternion.identity);
        proj.SetActive(true);

        float rnd = Random.Range(0f, driftProjectileRandomness);

        Vector3 dir = -transform.forward * (1 - ((driftProjectileRandomness / 2f) - rnd));
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
