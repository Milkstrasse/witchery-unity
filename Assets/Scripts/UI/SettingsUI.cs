using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private SettingsManager manager;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private LocalizeStringEvent stringEvent;

    private void Start()
    {
        manager.OnLanguageUpdated += UpdateLanguage;

        musicSlider.value = AudioManager.singleton.GetMusicVolume();
        soundSlider.value = AudioManager.singleton.GetSoundVolume();

        musicSlider.enabled = true;
        soundSlider.enabled = true;
    }

    private void UpdateLanguage(string lang)
    {
        stringEvent.StringReference.SetReference("StringTable", lang);
    }

    private void OnDestroy()
    {
        manager.OnLanguageUpdated -= UpdateLanguage;
    }
}
