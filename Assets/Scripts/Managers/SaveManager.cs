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

        int deltaUnlocked = GlobalData.fighters.Length - savedData.unlocked.GetLength(0);
        if (deltaUnlocked > 0)
        {
            bool[,] unlocked = new bool[GlobalData.fighters.Length, GlobalData.fighters[0].outfits.Length];
            for (int i = 0; i < savedData.unlocked.GetLength(0); i++)
            {
                for (int j = 0; j < GlobalData.fighters[0].outfits.Length; j++)
                {
                    unlocked[i, j] = savedData.unlocked[i, j];
                }
            }

            savedData.unlocked = unlocked;
        }

        int deltaMissions = GlobalData.missions.Length - savedData.missions.Length;
        if (deltaMissions > 0)
        {
            bool[] missions = new bool[GlobalData.missions.Length];
            for (int i = 0; i < savedData.missions.Length; i++)
            {
                missions[i] = savedData.missions[i];
            }

            savedData.missions = missions;
        }

        if (deltaUnlocked > 0 || deltaMissions > 0)
        {
            SaveData();
        }

        return true;
    }

    public static void CreateNewData(Fighter[] fighters, Mission[] missions)
    {
        savedData = new SavedData();

        int fighterAmount = fighters.Length;
        savedData.unlocked = new bool[fighterAmount, fighters[0].outfits.Length];
        for (int i = 0; i < Math.Min(fighterAmount, 4); i++)
        {
            savedData.unlocked[i, 0] = true;
            savedData.unlocked[i, 1] = i < 2;
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
