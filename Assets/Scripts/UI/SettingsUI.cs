using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private SettingsManager manager;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private LocalizeStringEvent currLang;
    [SerializeField] private LocalizeStringEvent currTheme;
    [SerializeField] private Slider scaleSlider;
    [SerializeField] private Toggle animate;
    [SerializeField] private Toggle highlight;

    private void Start()
    {
        manager.OnLanguageUpdated += UpdateLanguage;
        manager.OnThemeUpdated += UpdateTheme;
        manager.OnSettingsReset += UpdateSettings;

        UpdateSettings();

        highlight.enabled = true;
        animate.enabled = true;
    }

    private void UpdateSettings()
    {
        musicSlider.value = AudioManager.singleton.GetMusicVolume() * 10f;
        soundSlider.value = AudioManager.singleton.GetSoundVolume() * 10f;

        musicSlider.enabled = true;
        soundSlider.enabled = true;

        currLang.StringReference.SetReference("StringTable", LocalizationSettings.SelectedLocale.Identifier.Code);
        currTheme.StringReference.SetReference("StringTable", GlobalData.themes[GlobalData.themeIndex].name);

        scaleSlider.value = GlobalData.uiScale * 100f;
        scaleSlider.enabled = true;

        highlight.isOn = GlobalData.highlightPlayable;
        animate.isOn = GlobalData.animateImpact;
    }

    private void UpdateLanguage(string lang)
    {
        currLang.StringReference.SetReference("StringTable", lang);
    }

    private void UpdateTheme(string theme)
    {
        currTheme.StringReference.SetReference("StringTable", theme);
    }

    private void OnDestroy()
    {
        manager.OnLanguageUpdated -= UpdateLanguage;
        manager.OnThemeUpdated -= UpdateTheme;
    }
}