using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public void ChangeMusicVolume(float sliderValue) => AudioManager.singleton.ChangeMusicVolume(sliderValue);

    public void ChangeSoundVolume(float sliderValue) => AudioManager.singleton.ChangeSoundVolume(sliderValue);

    public void ResetSettings()
    {
        ChangeMusicVolume(1f);
        ChangeSoundVolume(1f);

        GlobalManager.singleton.LoadScene("SettingsScene");
    }

    public void ReturnToMenu()
    {
        PlayerPrefs.SetFloat("music", AudioManager.singleton.GetMusicVolume());
        PlayerPrefs.SetFloat("sound", AudioManager.singleton.GetSoundVolume());
        PlayerPrefs.Save();

        GlobalManager.singleton.LoadScene("MenuScene");
    }
}
