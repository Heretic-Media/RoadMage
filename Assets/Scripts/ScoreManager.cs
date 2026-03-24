using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;
    private int multiplier = 1;
    private float internal_multiplier = 0;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreMultiplierText;
    public void AddScore(int value)
    {
        score += value * multiplier;
        internal_multiplier += 0.75f;
        UpdateScoreText();
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreText()
    {
        // updates the text object to reflect the current score
        string fmt = "000000";

        scoreText.text = score.ToString(fmt);
    }

    public void UpdateMultiplierText()
    {

        if (multiplier <= 1)
        {
            scoreMultiplierText.text = " ";
        }
        else
        {
            scoreMultiplierText.text = "x " + multiplier.ToString();
        }
    }

    private void FixedUpdate()
    {
        // decay of the score multiplier which decays faster the large it is
        internal_multiplier -= Time.fixedDeltaTime * (float)multiplier * 0.3f;
        internal_multiplier = Mathf.Max(internal_multiplier, 0);
        multiplier = 1 + (int)Mathf.Floor(internal_multiplier);
        UpdateMultiplierText();
    }

    public int getMultiplier()
    {
        return multiplier;
    }
}
