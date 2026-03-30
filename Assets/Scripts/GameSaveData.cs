using System;

[Serializable]
public class GameSaveData
{
    public int stars;
    public int totalScore;
    public int highScore;
    public int currentCharacter;
    public int currentAccessory;
    public int[] characterColours; // colour index per character (truck=0, car=1, van=2)
    public string saveTimestamp;

    public bool rewardsPicked;
    public int silverCategory;
    public int silverCharacter;
    public int silverIndex;
    public int goldCategory;
    public int goldCharacter;
    public int goldIndex;

    public GameSaveData()
    {
        stars = 0;
        totalScore = 0;
        highScore = 0;
        currentCharacter = 0;
        currentAccessory = 0;
        characterColours = new int[] { 0, 2, 7 }; // defaults: truck=0, car=2, van=7
        saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        rewardsPicked = false;
        silverCategory = -1;
        silverCharacter = -1;
        silverIndex = -1;
        goldCategory = -1;
        goldCharacter = -1;
        goldIndex = -1;
    }
}
