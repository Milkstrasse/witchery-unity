using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private SettingsManager manager;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private LocalizeStringEvent stringEvent;

    [SerializeField] private Toggle[] toggles;

    private void Start()
    {
        manager.OnLanguageUpdated += UpdateLanguage;

        musicSlider.value = AudioManager.singleton.GetMusicVolume();
        soundSlider.value = AudioManager.singleton.GetSoundVolume();

        musicSlider.enabled = true;
        soundSlider.enabled = true;

        toggles[0].isOn = GlobalSettings.lifeIsResource;
        toggles[1].isOn = GlobalSettings.noValueStack;
        toggles[2].isOn = GlobalSettings.noRegainResource;
        toggles[3].isOn = GlobalSettings.noCostNoMatch;
        toggles[4].isOn = GlobalSettings.startAndGainEnergy;
        toggles[5].isOn = GlobalSettings.setEnergy;
        toggles[6].isOn = GlobalSettings.effectDecay;
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
