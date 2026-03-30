using UnityEngine;

public static class CurrencyManager
{
    public static int stars;
    public static int totalScore;
    public static int highScore;

    static CurrencyManager()
    {
        LoadFromSave();
    }

    public static void LoadFromSave()
    {
        GameSaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            stars = data.stars;
            totalScore = data.totalScore;
            highScore = data.highScore;
        }
        else
        {
            stars = 0;
            totalScore = 0;
            highScore = 0;
        }
    }

    public static void AddStars(int amount)
    {
        stars += Mathf.Max(0, amount);
    }

    public static void AddToTotalScore(int amount)
    {
        totalScore += Mathf.Max(0, amount);
        if (totalScore > highScore)
        {
            highScore = totalScore;
        }
    }

    public static void ResetRunScore()
    {
        totalScore = 0;
    }
}
