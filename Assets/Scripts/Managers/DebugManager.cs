using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private Button moneyButton;
    [SerializeField] private TextMeshProUGUI data;
    private void Start()
    {
        #if UNITY_EDITOR
        moneyButton.interactable = true;
        #endif

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"times fought: {SaveManager.savedData.timesFought}\n");
        stringBuilder.Append($"times won: {SaveManager.savedData.timesWon}\n");
        stringBuilder.Append($"money spent: {SaveManager.savedData.moneySpent}\n");
        stringBuilder.Append($"self KOed: {SaveManager.savedData.selfKO}\n");
        stringBuilder.Append($"healed opponent: {SaveManager.savedData.healedOpponent}\n");
        stringBuilder.Append($"won with effect: {SaveManager.savedData.wonWithEffect}\n");

        data.text = stringBuilder.ToString();
    }

    public void AddMoney()
    {
        AudioManager.singleton.PlayPositiveSound();
        SaveManager.savedData.money = Math.Min(SaveManager.savedData.money + 100, 999999);
    }
    public void DeleteData()
    {
        AudioManager.singleton.PlayNegativeSound();

        SaveManager.DeleteData();
        GlobalManager.singleton.LoadScene("StartScene");
    }
    public void ReturnToMenu()
    {
        AudioManager.singleton.PlayStandardSound();
        GlobalManager.singleton.LoadScene("MenuScene");
    }
}
