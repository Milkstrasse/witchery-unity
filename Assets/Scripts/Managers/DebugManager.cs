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
        stringBuilder.Append($"times won going first: {SaveManager.savedData.timesWonFirst}\n");
        stringBuilder.Append($"money spent: {SaveManager.savedData.moneySpent}");

        data.text = stringBuilder.ToString();
    }

    public void AddMoney() => SaveManager.savedData.money += 100;
    public void DeleteData() => SaveManager.DeleteData();
    public void ReturnToMenu() => GlobalManager.singleton.LoadScene("MenuScene");
}
