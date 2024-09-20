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

     public event Action<string> OnLanguageUpdated;

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
                Debug.Log("kkkkkkkkkkkk");
                langIndex = 0;
            }
        }
        else
        {
            Debug.Log("jgljfdglfdjjglf");
            langIndex = 0;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[langIndex];
        
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
