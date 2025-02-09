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
                float percent = (fighterData.timesUsedPrimary + fighterData.timesUsedSecondary) / (float)SaveManager.savedData.timesFought;
                percent = Mathf.Min(percent, 1f);

                LeanTween.value(statBar.gameObject, statBar.fillAmount, percent, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });

                stat.text = $"{Mathf.RoundToInt(percent * 100f)}%";
            }
            else if (category == 1 && fighterData.timesUsedPrimary > 0)
            {
                float percent = fighterData.timesWonPrimary / (float)fighterData.timesUsedPrimary;
                percent = Mathf.Min(percent, 1f);

                LeanTween.value(statBar.gameObject, statBar.fillAmount, percent, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });

                stat.text = $"{Mathf.RoundToInt(percent * 100f)}%";
            }
            else if (category == 2 && fighterData.timesUsedPrimary > 0)
            {
                float percent = fighterData.timesWonSecondary / (float)fighterData.timesUsedSecondary;
                percent = Mathf.Min(percent, 1f);

                LeanTween.value(statBar.gameObject, statBar.fillAmount, percent, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });

                stat.text = $"{Mathf.RoundToInt(percent * 100f)}%";
            }
            else
            {
                stat.text = "0%";
                LeanTween.value(statBar.gameObject, statBar.fillAmount, 0f, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });
            }
        }
        else
        {
            stat.text = "0%";
            LeanTween.value(statBar.gameObject, statBar.fillAmount, 0f, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });
        }
    }
}
