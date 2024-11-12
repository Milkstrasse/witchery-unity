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

    public static void CreateNewData(Fighter[] fighters, int icon)
    {
        savedData.icon = icon;

        int fighterAmount = fighters.Length;

        savedData.unlocked = new bool[fighterAmount, fighters[0].outfits.Length];
        for (int i = 0; i < fighterAmount; i++)
        {
            savedData.unlocked[i, 0] = true;

            if (i < 2)
            {
                savedData.unlocked[i, 1] = true;
            }
        }

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

    public static void DeleteData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }
}
