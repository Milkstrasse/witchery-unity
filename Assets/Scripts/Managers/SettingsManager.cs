using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToUpdate;

    private bool applying;
    private int langIndex;

    public event Action<string> OnLanguageUpdated;
    public event Action<string> OnThemeUpdated;
    public event Action OnSettingsReset;

    private void Start()
    {
        langIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
    }

    public void ChangeMusicVolume(float sliderValue) => AudioManager.singleton.ChangeMusicVolume(sliderValue);

    public void ChangeSoundVolume(float sliderValue) => AudioManager.singleton.ChangeSoundVolume(sliderValue);

    public void DecreaseLang()
    {
        if (applying)
            return;

        AudioManager.singleton.PlayStandardSound();

        if (langIndex > 0)
        {
            langIndex--;
        }
        else
        {
            langIndex = LocalizationSettings.AvailableLocales.Locales.Count - 1;
        }

        StartCoroutine(SetLocale(langIndex));
    }

    public void IncreaseLang()
    {
        if (applying)
            return;

        AudioManager.singleton.PlayStandardSound();

        if (langIndex < LocalizationSettings.AvailableLocales.Locales.Count - 1)
        {
            langIndex++;
        }
        else
        {
            langIndex = 0;
        }
        
        StartCoroutine(SetLocale(langIndex));
    }

    public void DecreaseTheme()
    {
        if (applying)
            return;

        AudioManager.singleton.PlayStandardSound();

        if (GlobalData.themeIndex > 0)
        {
            GlobalData.themeIndex--;
        }
        else
        {
            GlobalData.themeIndex = GlobalData.themes.Length - 1;
        }

        StartCoroutine(ChangeTheme());
    }

    public void IncreaseTheme()
    {
        if (applying)
            return;

        AudioManager.singleton.PlayStandardSound();

        if (GlobalData.themeIndex < GlobalData.themes.Length - 1)
        {
            GlobalData.themeIndex++;
        }
        else
        {
            GlobalData.themeIndex = 0;
        }
        
        StartCoroutine(ChangeTheme());
    }

    IEnumerator ChangeTheme()
    {
        applying = true;

        GlobalManager.singleton.ApplyTheme();
        yield return null;

        for (int i = 0; i < objectsToUpdate.Length; i++)
        {
            objectsToUpdate[i].SetActive(false);
            objectsToUpdate[i].SetActive(true);
        }

        OnThemeUpdated?.Invoke(GlobalData.themes[GlobalData.themeIndex].name);

        applying = false;
    }

    IEnumerator SetLocale(int localID)
    {
        applying = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localID];
        OnLanguageUpdated?.Invoke(LocalizationSettings.SelectedLocale.Identifier.Code);

        applying = false;
    }

    public void ResetSettings()
    {
        if (applying)
            return;

        ChangeMusicVolume(1f);
        ChangeSoundVolume(1f);

        AudioManager.singleton.PlayStandardSound();

        if (Application.systemLanguage != SystemLanguage.Unknown)
        {
            Locale systemLocale = LocalizationSettings.AvailableLocales.GetLocale(Application.systemLanguage);
            if (systemLocale != null)
            {
                langIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(systemLocale);
            }
            else
            {
                langIndex = 0;
            }
        }
        else
        {
            langIndex = 0;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[langIndex];
        GlobalData.themeIndex = 1;

        GlobalData.highlightPlayable = true;
        GlobalData.animateImpact = true;

        OnSettingsReset?.Invoke();

        StartCoroutine(ChangeTheme());
    }

    public void PlaySound()
    {
        AudioManager.singleton.PlayStandardSound();
    }

    public void HighlightCard(bool enable)
    {
        GlobalData.highlightPlayable = enable;
    }

    public void AnimateImpact(bool enable)
    {
        GlobalData.animateImpact = enable;
    }

    public bool SavingSettings()
    {
        if (applying)
            return false;

        PlayerPrefs.SetFloat("music", AudioManager.singleton.GetMusicVolume());
        PlayerPrefs.SetFloat("sound", AudioManager.singleton.GetSoundVolume());
        PlayerPrefs.SetInt("langCode", langIndex);
        PlayerPrefs.SetInt("highlightPlayable", GlobalData.highlightPlayable ? 1 : 0);
        PlayerPrefs.SetInt("animateImpact", GlobalData.animateImpact ? 1 : 0);
        PlayerPrefs.SetInt("theme", GlobalData.themeIndex);
        PlayerPrefs.Save();

        return true;
    }
}
