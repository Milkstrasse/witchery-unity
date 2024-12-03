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
    [SerializeField] private Toggle animate;
    [SerializeField] private Toggle highlight;

    private void Start()
    {
        manager.OnLanguageUpdated += UpdateLanguage;
        manager.OnThemeUpdated += UpdateTheme;

        musicSlider.value = AudioManager.singleton.GetMusicVolume();
        soundSlider.value = AudioManager.singleton.GetSoundVolume();

        musicSlider.enabled = true;
        soundSlider.enabled = true;

        currTheme.StringReference.SetReference("StringTable", GlobalData.themes[GlobalData.themeIndex].name);

        highlight.isOn = GlobalData.highlightPlayable;
        animate.isOn = GlobalData.animateImpact;
        highlight.enabled = true;
        animate.enabled = true;
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
