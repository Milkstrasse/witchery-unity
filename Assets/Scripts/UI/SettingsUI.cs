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

    [SerializeField] private Toggle[] toggles;

    private void Start()
    {
        manager.OnLanguageUpdated += UpdateLanguage;
        manager.OnThemeUpdated += UpdateTheme;

        musicSlider.value = AudioManager.singleton.GetMusicVolume();
        soundSlider.value = AudioManager.singleton.GetSoundVolume();

        musicSlider.enabled = true;
        soundSlider.enabled = true;

        toggles[0].isOn = GlobalSettings.lifeIsResource;
        toggles[1].isOn = GlobalSettings.noRegainResource;
        toggles[2].isOn = GlobalSettings.noCostNoMatch;
        toggles[3].isOn = GlobalSettings.startAndGainEnergy;

        currTheme.StringReference.SetReference("StringTable", GlobalManager.singleton.themes[GlobalSettings.themeIndex].name);
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
