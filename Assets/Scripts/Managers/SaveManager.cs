using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager
{
    string saveFilePath = Application.persistentDataPath + "/PlayerData.dat";

    public bool LoadData()
    {
        if (!File.Exists(saveFilePath))
            return false;

        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Open(saveFilePath, FileMode.Open);

        SavedData playerData = (SavedData)binary.Deserialize(file);
        file.Close();

        GlobalSettings.playerName = playerData.name;
        GlobalSettings.unlocked = playerData.unlocked;

        return true;
    }

    public void CreateNewData(Fighter[] fighters)
    {
        int fighterAmount = fighters.Length;

        GlobalSettings.unlocked = new bool[fighterAmount, fighters[0].outfits.Length];
        for (int i = 0; i < fighterAmount; i++)
        {
            GlobalSettings.unlocked[i, 0] = true;

            if (i < 2)
            {
                GlobalSettings.unlocked[i, 1] = true;
            }
        }

        SaveData();
    }

    private void SaveData()
    {
        SavedData playerData = new SavedData(GlobalSettings.playerName, GlobalSettings.unlocked);

        BinaryFormatter binary = new BinaryFormatter();
		FileStream file = File.Create(saveFilePath);                     

		binary.Serialize(file, playerData);
		file.Close();
    }

    public void DeleteData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }
}

[Serializable]
public struct SavedData
{
    public string name;
    public bool[,] unlocked;

    public SavedData(string name, bool[,] unlocked)
    {
        this.name = name;
        this.unlocked = unlocked;
    }
}
