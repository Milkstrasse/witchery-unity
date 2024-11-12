using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "ScriptableObjects/Mission")]
public class Mission : ScriptableObject
{
    public int reward;
    public int goalValue;
    public string checkVariable;

    public bool isClaimable;

    public void CheckStatus()
    {
        isClaimable = (int) SaveManager.savedData.GetType().GetField(checkVariable).GetValue(SaveManager.savedData) >= goalValue;
    }
}
