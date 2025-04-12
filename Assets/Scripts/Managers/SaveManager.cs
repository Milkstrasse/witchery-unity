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

        int deltaFighters = GlobalData.fighters.Length - savedData.fighters.Length;
        if (deltaFighters > 0)
        {
            FighterData[] fighters = new FighterData[GlobalData.fighters.Length];
            for (int i = 0; i < savedData.fighters.Length; i++)
            {
                fighters[i] = savedData.fighters[i];
            }
            for (int j = 0; j < deltaFighters; j++)
            {
                fighters[savedData.fighters.Length + j] = new FighterData(GlobalData.fighters[savedData.fighters.Length + j]);
            }

            savedData.fighters = fighters;
        }

        int deltaMissions = GlobalData.missions.Length - savedData.missions.Length;
        if (deltaMissions > 0)
        {
            bool[] missions = new bool[GlobalData.missions.Length];
            for (int i = 0; i < savedData.missions.Length; i++)
            {
                missions[i] = savedData.missions[i];
            }
            for (int j = 0; j < deltaFighters; j++)
            {
                missions[savedData.missions.Length + j] = GlobalData.missions[savedData.fighters.Length + j];
            }

            savedData.missions = missions;
        }

        if (deltaFighters > 0 || deltaMissions > 0)
        {
            SaveData();
        }

        return true;
    }

    public static void CreateNewData(Fighter[] fighters, Mission[] missions)
    {
        savedData = new SavedData();

        int fighterAmount = fighters.Length;
        savedData.fighters = new FighterData[fighterAmount];

        for (int i = 0; i < fighterAmount; i++)
        {
            savedData.fighters[i] = new FighterData(fighters[i]);

            if (i < Math.Min(fighterAmount, 4))
            {
                savedData.fighters[i].UnlockFighter();
                //savedData.fighters[i].SetOutfit(1, i < 2);
            }
        }

        savedData.missions = new bool[missions.Length];

        SaveData();
    }

    public static void SaveData()
    {
        BinaryFormatter binary = new BinaryFormatter();
		FileStream file = File.Create(saveFilePath);                     

		binary.Serialize(file, savedData);
		file.Close();
    }

    public static void UpdateStats(PlayerData playerData, bool gameHasEnded, PlayerObject player)
    {
        if (!savedData.healedOpponent)
        {
            savedData.healedOpponent = playerData.healedOpponent;
        }
        if (!savedData.nothingStolen)
        {
            savedData.nothingStolen = playerData.stoleNothing;
        }
        if (!savedData.wonWithEffect)
        {
            savedData.wonWithEffect = playerData.wonWithEffect;
        }

        savedData.maxEffectCount = Math.Max(playerData.maxEffects, savedData.maxEffectCount);

        if (playerData.playedUntilEnd || (gameHasEnded && playerData.roundsPlayed >= 3))
        {
            savedData.timesFought++;

            if (playerData.startedFirst)
            {
                savedData.timesFoughtFirst++;
            }

            if (player.hasWon)
            {
                savedData.fighters[player.fighterIDs[0].fighterID].IncreasePrimaryUse();
                savedData.fighters[player.fighterIDs[0].fighterID].timesWonPrimary++;

                for (int i = 1; i < player.fighterIDs.Length; i++)
                {
                    savedData.fighters[player.fighterIDs[i].fighterID].IncreaseSecondaryUse();
                    savedData.fighters[player.fighterIDs[i].fighterID].timesWonSecondary++;
                }

                if (playerData.startedFirst)
                {
                    savedData.timesWonFirst++;
                }

                savedData.timesWon++;
            }
            else
            {
                savedData.fighters[player.fighterIDs[0].fighterID].IncreasePrimaryUse();
                for (int i = 1; i < player.fighterIDs.Length; i++)
                {
                    savedData.fighters[player.fighterIDs[i].fighterID].IncreaseSecondaryUse();
                }
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
