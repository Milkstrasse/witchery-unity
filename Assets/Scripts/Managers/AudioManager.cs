using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager singleton;

    [SerializeField] AudioClip[] audioClips;
    private AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        singleton = this;

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayStandardSound()
    {
        audioSource.PlayOneShot(audioClips[0]);
    }
}
