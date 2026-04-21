using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static readonly string SavePath = Application.persistentDataPath + "/gamesave.json";

    public static void SaveGame()
    {
        GameSaveData saveData = new GameSaveData
        {
            stars = CurrencyManager.stars,
            totalScore = CurrencyManager.totalScore,
            highScore = CurrencyManager.highScore,
            currentCharacter = CharacterCustomisation.currentCharacter,
            currentAccessory = CharacterCustomisation.currentAccessory,
            //characterColours = new int[]
            //{
            //    CharacterCustomisation.truckColourIndex,
            //    CharacterCustomisation.carColourIndex,
            //    CharacterCustomisation.vanColourIndex
            //},
            //rewardsPicked = CharacterCustomisation.rewardsPicked,
            //silverCategory = CharacterCustomisation.silverCategory,
            //silverCharacter = CharacterCustomisation.silverCharacter,
            //silverIndex = CharacterCustomisation.silverIndex,
            //goldCategory = CharacterCustomisation.goldCategory,
            //goldCharacter = CharacterCustomisation.goldCharacter,
            //goldIndex = CharacterCustomisation.goldIndex
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game saved to: " + SavePath);
    }

    public static GameSaveData LoadGame()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("Game loaded from: " + SavePath);
            return saveData;
        }
        else
        {
            Debug.Log("No save file found at: " + SavePath);
            return null;
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }
}
