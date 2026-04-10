using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffects : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [Header("Effects")]
    [SerializeField] private GameObject healEffect;
    private bool healPulseActive;

    [SerializeField] private GameObject damageEffect;
    [SerializeField] private int damageThreshold = 1500;
    [SerializeField] private int crackedThreshold = 750;
    private bool damaged;
    private bool damagePulseActive;

    [SerializeField] private GameObject endStateEffect;

    [Header("Ability Effects")]
    [SerializeField] private GameObject iceEffect;

    [SerializeField] private GameObject fireEffect;

    [SerializeField] private GameObject kineticEffect;

    private GameObject[] effects;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {     
            player = GameObject.FindGameObjectWithTag("Player"); 
        }

        effects = new[]
        {
            healEffect,
            damageEffect,
            endStateEffect,
            iceEffect,
            fireEffect,
            kineticEffect
        };

        foreach (GameObject effect in effects)
        {
            if (effect != null)
            {
                effect.SetActive(false);
            }
        }

    }

    //damage effect
    public void TriggerDamageEffect()
    {
        bool isLowHealth = player.GetComponent<Health>().health <= damageThreshold;

        if (isLowHealth)
        {
            PulseFade(damageEffect, damagePulseActive, 0.13f);
        }
        if (isLowHealth && !damaged)
        {
            damaged = true;
            damagePulseActive = false;
            DamagePulseOn();
        }
        else if (!isLowHealth && damaged)
        {
            CancelInvoke(nameof(DamagePulseOn));
            CancelInvoke(nameof(DamagePulseOff));
            damageEffect.SetActive(false);
            damaged = false;
            damagePulseActive = false;
        }

        bool cracked = player.GetComponent<Health>().health <= crackedThreshold;

        if (cracked)
        {
            damageEffect.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (!cracked)
        {
            damageEffect.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void DamagePulseOff()
    {
        if (!damaged)
        {
            return;
        }

        var pulse = damageEffect.transform.GetChild(1);
        pulse.gameObject.SetActive(false);
        damagePulseActive = false;
        Invoke(nameof(DamagePulseOn), 2f);
    }

    private void DamagePulseOn()
    {
        if (!damaged || damagePulseActive)
        {
            return;
        }

        damageEffect.SetActive(true);
        var pulse = damageEffect.transform.GetChild(1);
        pulse.gameObject.SetActive(true);
        damagePulseActive = true;
        Invoke(nameof(DamagePulseOff), 2f);
    }


    private void PulseFade(GameObject target, bool condition, float maxAlpha)
    {
        var pulse = target.transform.GetChild(1).GetComponent<Image>();

        var colour = pulse.color;
        float fadeSpeed = 0.1f;

        if (condition)
        {
            colour.a = Mathf.Clamp01(colour.a - Time.deltaTime * fadeSpeed);
        }
        else
        {
            colour.a = Mathf.Min(colour.a + Time.deltaTime * fadeSpeed, maxAlpha);
        }

        pulse.color = colour;
    }

    // heal effect
    public void TriggerHealEffect()
    {
        healEffect.SetActive(true);
        healPulseActive = true;
        Invoke("StopHealEffect", 1f);
    }

    private void StopHealEffect()
    {
        healPulseActive = false;
        healEffect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        TriggerDamageEffect();

        if (healPulseActive)
        {
            PulseFade(healEffect, healPulseActive, 0.063f);
        }
    }
}
