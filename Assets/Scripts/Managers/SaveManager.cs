using System.IO;
using UnityEngine;

public class SaveManager
{
    string saveFilePath = Application.persistentDataPath + "/PlayerData.json";
    
    struct PlayerData
    {
        public string name;

        public PlayerData(string name)
        {
            this.name = name;
        }
    }

    public bool LoadData()
    {
        if (!File.Exists(saveFilePath))
            return false;

        string loadData = File.ReadAllText(saveFilePath);
        
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(loadData);
        GlobalSettings.playerName = playerData.name;

        return true;
    }

    public void SaveData()
    {
        PlayerData playerData = new PlayerData(GlobalSettings.playerName);

        string saveData = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath, saveData);
    }

    public void DeleteData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }
}
