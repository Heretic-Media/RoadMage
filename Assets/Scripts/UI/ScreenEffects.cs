using UnityEngine;
using UnityEngine.UI;

public class ScreenEffects : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [Header("Effects")]
    [SerializeField] private GameObject healEffect;
    [SerializeField] private GameObject damageEffect;
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
        bool isLowHealth = player.GetComponent<Health>().health <= 1500;

        if (isLowHealth)
        {
            PulseFade();
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

        bool cracked = player.GetComponent<Health>().health <= 750;

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


    private void PulseFade()
    {
        var pulse = damageEffect.transform.GetChild(1).GetComponent<Image>();

        var colour = pulse.color;
        float fadeSpeed = 0.1f;

        if (damagePulseActive)
        {
            colour.a = Mathf.Clamp01(colour.a - Time.deltaTime * fadeSpeed);
        }
        else
        {
            colour.a = Mathf.Min(colour.a + Time.deltaTime * fadeSpeed, 0.13f);
        }

        pulse.color = colour;
    }

    // Update is called once per frame
    void Update()
    {
        TriggerDamageEffect();

    }
}
