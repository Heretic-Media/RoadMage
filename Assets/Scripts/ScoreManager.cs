using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;

    [SerializeField] private TextMeshProUGUI scoreText;

    public void AddScore(int value)
    {
        score += value;
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
}
