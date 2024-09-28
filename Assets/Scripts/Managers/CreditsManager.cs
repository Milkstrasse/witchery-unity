using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public void ReturnToMenu()
    {
        AudioManager.singleton.PlayStandardSound();
        GlobalManager.singleton.LoadScene("MenuScene");
    }
}
