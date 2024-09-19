using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager singleton;

    [SerializeField] AudioClip[] audioClips;
    [SerializeField] AudioSource musicSource;
    private AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        singleton = this;

        audioSource = GetComponent<AudioSource>();

        ChangeMusicVolume(PlayerPrefs.GetFloat("music", 1f));
        ChangeSoundVolume(PlayerPrefs.GetFloat("sound", 1f));
    }

    public void PlayStandardSound()
    {
        audioSource.PlayOneShot(audioClips[0]);
    }

    public void ChangeMusicVolume(float sliderValue) => musicSource.volume = sliderValue;

    public void ChangeSoundVolume(float sliderValue) => audioSource.volume = sliderValue;

    public float GetMusicVolume()
    {
        return musicSource.volume;
    }

    public float GetSoundVolume()
    {
        return audioSource.volume;
    }
}
