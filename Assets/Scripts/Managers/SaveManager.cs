using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager
{
    private static readonly string saveFilePath = Application.persistentDataPath + "/PlayerData.dat";

    public static bool LoadData()
    {
        if (!File.Exists(saveFilePath))
            return false;

        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Open(saveFilePath, FileMode.Open);

        SavedData playerData = (SavedData)binary.Deserialize(file);
        file.Close();

        GlobalSettings.playerName = playerData.name;
        GlobalSettings.money = playerData.money;
        GlobalSettings.unlocked = playerData.unlocked;

        return true;
    }

    public static void CreateNewData(Fighter[] fighters)
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

    public static void SaveData()
    {
        SavedData playerData = new SavedData(GlobalSettings.playerName, GlobalSettings.money, GlobalSettings.unlocked);

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
