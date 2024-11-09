using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SettingsManager : MonoBehaviour
{
    private bool changingLang;
    private int langIndex;
    private bool changingTheme;

    public event Action<string> OnLanguageUpdated;
    public event Action<string> OnThemeUpdated;

    private void Start()
    {
        langIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        OnLanguageUpdated?.Invoke(LocalizationSettings.SelectedLocale.Identifier.Code);
    }

    public void ChangeMusicVolume(float sliderValue) => AudioManager.singleton.ChangeMusicVolume(sliderValue);

    public void ChangeSoundVolume(float sliderValue) => AudioManager.singleton.ChangeSoundVolume(sliderValue);

    public void DecreaseLang()
    {
        if (changingLang)
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
        if (changingLang)
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
        if (changingTheme)
            return;

        AudioManager.singleton.PlayStandardSound();

        if (GlobalSettings.themeIndex > 0)
        {
            GlobalSettings.themeIndex--;
        }
        else
        {
            GlobalSettings.themeIndex = GlobalManager.singleton.themes.Length - 1;
        }

        StartCoroutine(ChangeTheme());
    }

    public void IncreaseTheme()
    {
        if (changingTheme)
            return;

        AudioManager.singleton.PlayStandardSound();

        if (GlobalSettings.themeIndex < GlobalManager.singleton.themes.Length - 1)
        {
            GlobalSettings.themeIndex++;
        }
        else
        {
            GlobalSettings.themeIndex = 0;
        }
        
        StartCoroutine(ChangeTheme());
    }

    IEnumerator ChangeTheme()
    {
        changingTheme = true;

        GlobalManager.singleton.ApplyTheme(GlobalSettings.themeIndex);
        yield return null;

        OnThemeUpdated?.Invoke(GlobalManager.singleton.themes[GlobalSettings.themeIndex].name);

        changingTheme = false;
    }

    IEnumerator SetLocale(int localID)
    {
        changingLang = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localID];
        OnLanguageUpdated?.Invoke(LocalizationSettings.SelectedLocale.Identifier.Code);

        changingLang = false;
    }

    public void ToggleLifeResource(bool toggleValue) => GlobalSettings.lifeIsResource = toggleValue;
    public void ToggleRegainResource(bool toggleValue) => GlobalSettings.noRegainResource = toggleValue;
    public void ToggleCostMatch(bool toggleValue) => GlobalSettings.noCostNoMatch = toggleValue;
    public void ToggleEnergy(bool toggleValue) => GlobalSettings.startAndGainEnergy = toggleValue;

    public void ResetSettings()
    {
        if (changingLang || changingTheme)
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

        GlobalSettings.lifeIsResource = false;
        GlobalSettings.noRegainResource = false;
        GlobalSettings.noCostNoMatch = false;
        GlobalSettings.startAndGainEnergy = false;
        GlobalSettings.themeIndex = 1;

        GlobalManager.singleton.LoadScene("SettingsScene");

        StartCoroutine(ChangeTheme());
    }

    public void ReturnToMenu()
    {
        if (changingLang || changingTheme)
            return;

        AudioManager.singleton.PlayStandardSound();

        PlayerPrefs.SetFloat("music", AudioManager.singleton.GetMusicVolume());
        PlayerPrefs.SetFloat("sound", AudioManager.singleton.GetSoundVolume());
        PlayerPrefs.SetInt("langCode", langIndex);
        PlayerPrefs.SetInt("theme", GlobalSettings.themeIndex);
        PlayerPrefs.Save();

        GlobalManager.singleton.LoadScene("MenuScene");
    }
}
