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

        SavedData savedData = (SavedData)binary.Deserialize(file);
        file.Close();

        GlobalSettings.playerName = savedData.name;
        GlobalSettings.icon = savedData.icon;
        GlobalSettings.money = savedData.money;
        GlobalSettings.unlocked = savedData.unlocked;
        GlobalSettings.shopFighters = savedData.shopFighters;

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

        GlobalSettings.shopFighters = new SelectedFighter[0];

        SaveData();
    }

    public static void SaveData()
    {
        SavedData savedData = new SavedData(GlobalSettings.playerName, GlobalSettings.icon, GlobalSettings.money, GlobalSettings.unlocked, GlobalSettings.shopFighters);

        BinaryFormatter binary = new BinaryFormatter();
		FileStream file = File.Create(saveFilePath);                     

		binary.Serialize(file, savedData);
		file.Close();
    }

    public static void DeleteData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }
}
