using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenManager : MonoBehaviour
{
    [SerializeField] private Image starFillA;
    [SerializeField] private Image starFillB;
    [SerializeField] private Image starFillC;

    [SerializeField] private GameObject winScreen;

    [SerializeField] private GameObject scoreText;

    [SerializeField] private int secondStarThreshold = 1000;
    [SerializeField] private bool secondStarUnlock = false;
    [SerializeField] private string secondStarUnlockName = "";
    [SerializeField] private int thirdStarThreshold = 3000;
    [SerializeField] private bool thirdStarUnlock = false;
    [SerializeField] private string thirdStarUnlockName = "";


    private Vector2 scoreTextDefaultPos;

    private ScoreManager scoreManager;

    public void ChangeWinScreenVisibility(bool visible)
    {
        winScreen.SetActive(visible);
    }

    public void ChangeStarFill(int amount)
    {
        starFillA.enabled = false;
        starFillB.enabled = false;
        starFillC.enabled = false;

        if (amount >= 1)
        {
            starFillA.enabled = true;
        }
        if (amount >= 2 && scoreManager.GetScore() >= secondStarThreshold)
        {
            starFillB.enabled = true;
            if (secondStarUnlock)
            {
                GameObject.FindGameObjectWithTag("UnlockManager").GetComponent<UnlocksForLevel>().addUnlockedItem(secondStarUnlockName);
            }
        }
        if (amount >= 3 && scoreManager.GetScore() >= thirdStarThreshold)
        {
            starFillC.enabled = true;
            if (thirdStarUnlock)
            {
                GameObject.FindGameObjectWithTag("UnlockManager").GetComponent<UnlocksForLevel>().addUnlockedItem(thirdStarUnlockName);
            }
        }
    }

    public void EneableScoreText()
    {
        scoreText.GetComponent<TextMeshProUGUI>().enabled = true;
        int score = scoreManager.GetScore();

        // updates the text object to reflect the current score
        string fmt = "000000";

        scoreText.GetComponent<TextMeshProUGUI>().text = score.ToString(fmt);
    }

    private void Awake()
    {
        scoreTextDefaultPos = scoreText.GetComponent<RectTransform>().anchoredPosition;
        scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
    }

    private void FixedUpdate()
    {
        scoreText.GetComponent<RectTransform>().anchoredPosition = scoreTextDefaultPos + Vector2.up * Mathf.Sin(Time.time) * 10f;

        starFillA.rectTransform.localScale = Vector3.one + Vector3.one * Mathf.Abs(Mathf.Cos(Time.time * 5)) * 0.1f;
        starFillB.rectTransform.localScale = Vector3.one + Vector3.one * Mathf.Abs(Mathf.Cos(Time.time * 5)) * 0.1f;
        starFillC.rectTransform.localScale = Vector3.one + Vector3.one * Mathf.Abs(Mathf.Cos(Time.time * 5)) * 0.1f;
    }
}
