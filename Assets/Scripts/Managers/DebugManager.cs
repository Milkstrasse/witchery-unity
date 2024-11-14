using System.Text;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI data;
    private void Start()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(SaveManager.savedData.name);
        stringBuilder.Append("\n");
        stringBuilder.Append($"times fought: {SaveManager.savedData.timesFought}\n");
        stringBuilder.Append($"times won: {SaveManager.savedData.timesWon}\n");
        stringBuilder.Append($"money spent: {SaveManager.savedData.moneySpent}\n");
        stringBuilder.Append($"self KOed: {SaveManager.savedData.selfKO}\n");
        stringBuilder.Append($"healed opponent: {SaveManager.savedData.healedOpponent}\n");
        stringBuilder.Append($"won with effect: {SaveManager.savedData.wonWithEffect}\n");

        data.text = stringBuilder.ToString();
    }

    public void AddMoney() => SaveManager.savedData.money += 100;
    public void DeleteData()
    {
        SaveManager.DeleteData();
        GlobalManager.singleton.LoadScene("StartScene");
    }
    public void ReturnToMenu() => GlobalManager.singleton.LoadScene("MenuScene");
}
