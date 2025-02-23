using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class StatisticsUI : MonoBehaviour
{
    [SerializeField] private GameObject statPrefab;
    [SerializeField] private Transform statsParent;

    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private LocalizeStringEvent categoryText;

    private FighterStat[] fighters;

    private string[] categories = new string[] {"useRate", "primaryRate", "secondaryRate", "primaryWin", "secondaryWin"};
    private int currCategory;

    public void Start()
    {
        fighters = new FighterStat[SaveManager.savedData.fighters.Length];

        for (int i = 0; i < SaveManager.savedData.fighters.Length; i++)
        {
            fighters[i] = Instantiate(statPrefab, statsParent).GetComponent<FighterStat>();
            fighters[i].SetupUI(SaveManager.savedData.fighters[i], i, 0);
        }

        playerText.text = $"times fought: {SaveManager.savedData.timesFought}\ntimes won: {SaveManager.savedData.timesWon}\ntimes won as first: {SaveManager.savedData.timesWonFirst}";
    }

    public void DecreaseCategory()
    {
        AudioManager.singleton.PlayStandardSound();

        if (currCategory > 0)
        {
            currCategory--;
        }
        else
        {
            currCategory = categories.Length - 1;
        }

        UpdateUI();
    }

    public void IncreaseCategory()
    {
        AudioManager.singleton.PlayStandardSound();

        if (currCategory < categories.Length - 1)
        {
            currCategory++;
        }
        else
        {
            currCategory = 0;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        categoryText.StringReference.SetReference("StringTable", categories[currCategory]);

        for (int i = 0; i < fighters.Length; i++)
        {
            fighters[i].SetupUI(SaveManager.savedData.fighters[i], i, currCategory);
        }
    }
}
