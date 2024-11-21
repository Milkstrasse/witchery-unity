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

    public void CheckStatus()
    {
        if (goalValue > 0)
        {
            isClaimable = (int) SaveManager.savedData.GetType().GetField(checkVariable).GetValue(SaveManager.savedData) >= goalValue;
        }
        else
        {
            isClaimable = (bool) SaveManager.savedData.GetType().GetField(checkVariable).GetValue(SaveManager.savedData);
        }
    }

    public enum Category
    {
        One, Two
    }
}
