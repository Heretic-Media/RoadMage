using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Space]
    [Header(" PLEASE READ ")]
    [Space]
    [Header("This works functionally, however the UI has not been setup yet.")]
    [Header("this can be done later, but is ready for alpha.")]
    [Header("Thank you :) - Lewis")]
    [Space(25)]

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    [SerializeField] Health health;

    [Header("XP / Level")]
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("Score")]
    public int score = 0;

    [Header("UI - Bars")]
    [Tooltip("Image with Fill Method set to filled for health.")]
    public Image healthBarFill;
    [Tooltip("Image with Fill Method set to filled for XP.")]
    public Image xpBarFill;

    [Header("UI - Optional Text")]
    public Text healthText;
    public Text xpText;
    public Text levelText;
    public Text scoreText;

    [SerializeField] private AudioSource hurtSound;

    private GameObject healthFuelGauge;

    void Start()
    {
        healthFuelGauge = GameObject.FindGameObjectWithTag("HealthFuelGauge");

        //currentHealth = maxHealth;
        UpdateAllUI();
    }

    public void AddScore(int amount)
    {
        score += Mathf.Max(0, amount);
        UpdateScoreUI();
    }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateXPUI();
    }

    public void ToggleTrails(bool on_off)
    {
        foreach (TrailRenderer trail in GetComponentsInChildren<TrailRenderer>())
        {
            trail.enabled = on_off;
        }
    }

    // May be removed eventually
    public void TakeDamage(int amount)
    {
        health.TakeDamage(amount);

        hurtSound.Play();

        /*if (currentHealth <= 0)
        {
            Die();
        }*/
    }

    void LevelUp()
    {
        level++;

        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.25f);

        UpdateLevelUI();
        UpdateXPUI();
        UpdateHealthUI();
    }

    public void Die()
    {

        Debug.Log("Player died.");

        SceneManager.LoadScene("GameOver");
    }

    void UpdateAllUI()
    {
        UpdateHealthUI();
        UpdateXPUI();
        UpdateLevelUI();
        UpdateScoreUI();
    }

    public void UpdateHealthUI()
    {
        healthFuelGauge.GetComponent<UIDialBehaviour>().UpdateGauge((float)health.health / (float)health.maxHealth);
        //if (healthBarFill != null)
        //{
        //    float t = (maxHealth > 0f) ? currentHealth / maxHealth : 0f;
        //    healthBarFill.fillAmount = Mathf.Clamp01(t);
        //}

        //if (healthText != null)
        //{
        //    healthText.text = $"{Mathf.CeilToInt(currentHealth)}/{Mathf.CeilToInt(maxHealth)}";
        //}
    }

    void UpdateXPUI()
    {
        if (xpBarFill != null)
        {
            float t = (xpToNextLevel > 0) ? (float)currentXP / xpToNextLevel : 0f;
            xpBarFill.fillAmount = Mathf.Clamp01(t);
        }

        if (xpText != null)
        {
            xpText.text = $"{currentXP}/{xpToNextLevel}";
        }
    }

    void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = $"LVL {level}";
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }


}