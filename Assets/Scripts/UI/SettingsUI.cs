using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private SettingsManager manager;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private LocalizeStringEvent stringEvent;

    [SerializeField] private Toggle toggle1;
    [SerializeField] private Toggle toggle2;
    [SerializeField] private Toggle toggle3;
    [SerializeField] private Toggle toggle4;

    private void Start()
    {
        manager.OnLanguageUpdated += UpdateLanguage;

        musicSlider.value = AudioManager.singleton.GetMusicVolume();
        soundSlider.value = AudioManager.singleton.GetSoundVolume();

        musicSlider.enabled = true;
        soundSlider.enabled = true;

        toggle1.isOn = GlobalSettings.setEnergy;
        toggle2.isOn = GlobalSettings.lifeIsResource;
        toggle3.isOn = GlobalSettings.stackEffectValue;
        toggle4.isOn = GlobalSettings.regainHP;
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
