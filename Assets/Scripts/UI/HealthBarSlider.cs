using UnityEngine;
using UnityEngine.UI;

public class HealthBarSlider : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject healthBarFill;
    [SerializeField] private GameObject healthBarGauge;
    private float healthToDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        UpdateHealthDisplay(0);
    }

    public void UpdateHealthDisplay(float health)
    {

        //healthToDisplay = -health;
        var slider = healthBarFill.GetComponent<Slider>();
        slider.value = health;

        //var fillRect = healthBarFill.GetComponent<RectTransform>();
       // var gaugeRect = healthBarGauge.GetComponent<RectTransform>();

       // var corners = new Vector3[4];
       // fillRect.GetWorldCorners(corners);

       // var topCenter = (corners[1] + corners[2]) * 0.5f;
       // gaugeRect.position = topCenter;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthDisplay(player.GetComponent<Health>().maxHealth - player.GetComponent<Health>().health);
    }
}
