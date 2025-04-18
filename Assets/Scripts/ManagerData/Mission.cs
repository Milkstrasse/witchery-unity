using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "ScriptableObjects/Mission")]
public class Mission : ScriptableObject
{
    public string descrKey;

    public int reward;
    public int goalValue;
    public string checkVariable;

    public bool isClaimable;
    public Category category;

    public int CheckStatus(int index)
    {
        if (goalValue > 0)
        {
            isClaimable = (int)SaveManager.savedData.GetType().GetField(checkVariable).GetValue(SaveManager.savedData) >= goalValue;
        }
        else
        {
            isClaimable = (bool)SaveManager.savedData.GetType().GetField(checkVariable).GetValue(SaveManager.savedData);
        }

        if (isClaimable && !SaveManager.savedData.missions[index] && category == Category.Fighter && !SaveManager.savedData.missions[index])
        {
            SaveManager.savedData.missions[index] = true;
            SaveManager.savedData.fighters[reward].UnlockFighter();

            SaveManager.SaveData();

            return 3;
        }
        else
        {
            return isClaimable && !SaveManager.savedData.missions[index] ? 4 : 0;
        }
    }

    public enum Category
    {
        Mission, Fighter
    }
}