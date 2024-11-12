using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private SettingsManager manager;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private LocalizeStringEvent currLang;
    [SerializeField] private LocalizeStringEvent currTheme;

    private void Start()
    {
        manager.OnLanguageUpdated += UpdateLanguage;
        manager.OnThemeUpdated += UpdateTheme;

        musicSlider.value = AudioManager.singleton.GetMusicVolume();
        soundSlider.value = AudioManager.singleton.GetSoundVolume();

        musicSlider.enabled = true;
        soundSlider.enabled = true;

        currTheme.StringReference.SetReference("StringTable", GlobalData.themes[GlobalData.themeIndex].name);
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
