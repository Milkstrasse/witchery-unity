using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SettingsManager : MonoBehaviour
{
    private bool changingLang;
    private int langIndex;
    private int themeIndex;

    [SerializeField] Material[] materials;

    public event Action<string> OnLanguageUpdated;
    public event Action<string> OnThemeUpdated;

    private void Start()
    {
        langIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        OnLanguageUpdated?.Invoke(LocalizationSettings.SelectedLocale.Identifier.Code);

        themeIndex = 1;
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
        AudioManager.singleton.PlayStandardSound();

        if (themeIndex > 0)
        {
            themeIndex--;
        }
        else
        {
            themeIndex = GlobalManager.singleton.themes.Length - 1;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = GlobalManager.singleton.themes[themeIndex].colors[i];
        }

        OnThemeUpdated?.Invoke(GlobalManager.singleton.themes[themeIndex].name);
    }

    public void IncreaseTheme()
    {
        AudioManager.singleton.PlayStandardSound();

        if (themeIndex < GlobalManager.singleton.themes.Length - 1)
        {
            themeIndex++;
        }
        else
        {
            themeIndex = 0;
        }
        
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = GlobalManager.singleton.themes[themeIndex].colors[i];
        }

        OnThemeUpdated?.Invoke(GlobalManager.singleton.themes[themeIndex].name);
    }

    IEnumerator SetLocale(int localID)
    {
        changingLang = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localID];
        OnLanguageUpdated?.Invoke(LocalizationSettings.SelectedLocale.Identifier.Code);

        changingLang = false;
    }

    public void ResetSettings()
    {
        ChangeMusicVolume(1f);
        ChangeSoundVolume(1f);

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
        themeIndex = 1;
        
        GlobalManager.singleton.LoadScene("SettingsScene");
    }

    public void ReturnToMenu()
    {
        AudioManager.singleton.PlayStandardSound();

        PlayerPrefs.SetFloat("music", AudioManager.singleton.GetMusicVolume());
        PlayerPrefs.SetFloat("sound", AudioManager.singleton.GetSoundVolume());
        PlayerPrefs.SetInt("langCode", langIndex);
        PlayerPrefs.Save();

        GlobalManager.singleton.LoadScene("MenuScene");
    }
}
