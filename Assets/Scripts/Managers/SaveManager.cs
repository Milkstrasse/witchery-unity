using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager
{
    private static readonly string saveFilePath = Application.persistentDataPath + "/PlayerData.dat";

    public static SavedData savedData;

    public static bool LoadData()
    {
        if (!File.Exists(saveFilePath))
            return false;

        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Open(saveFilePath, FileMode.Open);

        savedData = (SavedData)binary.Deserialize(file);
        file.Close();

        return true;
    }

    public static void CreateNewData(Fighter[] fighters, Mission[] missions)
    {
        int fighterAmount = fighters.Length;
        savedData.unlocked = new bool[fighterAmount, fighters[0].outfits.Length];
        for (int i = 0; i < Math.Min(fighterAmount, 8); i++)
        {
            savedData.unlocked[i, 0] = true;
        }

        savedData.missions = new bool[missions.Length];
        
        savedData.shopFighters = new SelectedFighter[0];

        SaveData();
    }

    public static void SaveData()
    {
        BinaryFormatter binary = new BinaryFormatter();
		FileStream file = File.Create(saveFilePath);                     

		binary.Serialize(file, savedData);
		file.Close();
    }

    public static void UpdateStats(PlayerData playerData, bool gameHasEnded, bool hasWon)
    {
        if (!savedData.healedOpponent)
        {
            savedData.healedOpponent = playerData.healedOpponent;
        }
        if (!savedData.nothingStolen)
        {
            savedData.nothingStolen = playerData.stoleNothing;
        }
        if (!savedData.selfKO)
        {
            savedData.selfKO = playerData.selfKO;
        }
        if (!savedData.wonWithEffect)
        {
            savedData.wonWithEffect = playerData.wonWithEffect;
        }

        savedData.maxEffectCount = Math.Max(playerData.maxEffects, savedData.maxEffectCount);

        if (playerData.playedUntilEnd || (gameHasEnded && playerData.roundsPlayed >= 3))
        {
            savedData.timesFought++;

            if (hasWon)
            {
                savedData.timesWon++;
                savedData.money = Math.Min(savedData.money + 25, 999999);
            }
            else
            {
                savedData.money = Math.Min(savedData.money + 5, 999999);
            }
        }
    }

    public static void DeleteData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }
}
