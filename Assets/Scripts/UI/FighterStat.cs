using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FighterStat : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private Image statBar;
    [SerializeField] private TextMeshProUGUI stat;

    public void SetupUI(FighterData fighterData, int fighterID, int category)
    {
        portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalData.fighters[fighterID].name + "-" + GlobalData.fighters[fighterID].outfits[0].name);

        if (SaveManager.savedData.timesFought > 0)
        {
            if (category == 0)
            {
                float percent = fighterData.timesWonPrimary/(float) SaveManager.savedData.timesFought;
                percent = Mathf.Min(percent, 1f);

                LeanTween.value(statBar.gameObject, statBar.fillAmount, percent, 0.3f).setOnUpdate( (float val) => { statBar.fillAmount = val; } );

                stat.text = $"{Mathf.RoundToInt(percent * 100f)}%";
            }
            else
            {
                float percent = fighterData.timesWonSecondary/(float) SaveManager.savedData.timesFought;
                percent = Mathf.Min(percent, 1f);
                
                LeanTween.value(statBar.gameObject, statBar.fillAmount, percent, 0.3f).setOnUpdate( (float val) => { statBar.fillAmount = val; } );

                stat.text = $"{Mathf.RoundToInt(percent * 100f)}%";
            }
        }
        else
        {
            stat.text = "0%";
        }
    }
}
