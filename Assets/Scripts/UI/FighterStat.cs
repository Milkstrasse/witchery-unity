using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class FighterStat : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private Image statBar;
    [SerializeField] private TextMeshProUGUI stat;

    public void SetupUI(FighterData fighterData, int fighterID, int category)
    {
        AsyncOperationHandle<Sprite> AssetHandle = Addressables.LoadAssetAsync<Sprite>(GlobalData.fighters[fighterID].name + "-" + GlobalData.fighters[fighterID].outfits[0].name);
        AssetHandle.Completed += Sprite_Completed;

        if (SaveManager.savedData.timesFought > 0)
        {
            float percent;

            switch (category)
            {
                case 0: //used
                    percent = (fighterData.timesUsedPrimary + fighterData.timesUsedSecondary) / (float)SaveManager.savedData.timesFought;
                    percent = Mathf.Min(percent, 1f);

                    LeanTween.value(statBar.gameObject, statBar.fillAmount, percent, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });

                    stat.text = $"{Mathf.RoundToInt(percent * 100f)}%";

                    break;
                case 1: //used primary
                    int timesUsed1 = fighterData.timesUsedPrimary + fighterData.timesUsedSecondary;

                    if (timesUsed1 > 0)
                    {
                        percent = fighterData.timesUsedPrimary / (float)timesUsed1;
                        percent = Mathf.Min(percent, 1f);

                        LeanTween.value(statBar.gameObject, statBar.fillAmount, percent, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });

                        stat.text = $"{Mathf.RoundToInt(percent * 100f)}%";

                        break;
                    }
                    else
                    {
                        goto default;
                    }
                case 2: //used secondary
                    int timesUsed2 = fighterData.timesUsedPrimary + fighterData.timesUsedSecondary;

                    if (timesUsed2 > 0)
                    {
                        percent = fighterData.timesUsedSecondary / (float)timesUsed2;
                        percent = Mathf.Min(percent, 1f);

                        LeanTween.value(statBar.gameObject, statBar.fillAmount, percent, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });

                        stat.text = $"{Mathf.RoundToInt(percent * 100f)}%";

                        break;
                    }
                    else
                    {
                        goto default;
                    }
                case 3: //won primary
                    if (fighterData.timesUsedPrimary > 0)
                    {
                        percent = fighterData.timesWonPrimary / (float)fighterData.timesUsedPrimary;
                        percent = Mathf.Min(percent, 1f);

                        LeanTween.value(statBar.gameObject, statBar.fillAmount, percent, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });

                        stat.text = $"{Mathf.RoundToInt(percent * 100f)}%";

                        break;
                    }
                    else
                    {
                        goto default;
                    }
                case 4: //won secondary
                    if (fighterData.timesUsedSecondary > 0)
                    {
                        percent = fighterData.timesWonSecondary / (float)fighterData.timesUsedSecondary;
                        percent = Mathf.Min(percent, 1f);

                        LeanTween.value(statBar.gameObject, statBar.fillAmount, percent, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });

                        stat.text = $"{Mathf.RoundToInt(percent * 100f)}%";

                        break;
                    }
                    else
                    {
                        goto default;
                    }
                default:
                    stat.text = "-";
                    LeanTween.value(statBar.gameObject, statBar.fillAmount, 0f, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });

                    break;

            }
        }
        else
        {
            stat.text = "-";
            LeanTween.value(statBar.gameObject, statBar.fillAmount, 0f, 0.3f).setOnUpdate((float val) => { statBar.fillAmount = val; });
        }
    }

    private void Sprite_Completed(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            portrait.sprite = handle.Result;
        }
    }
}