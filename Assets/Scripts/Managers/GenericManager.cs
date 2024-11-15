using UnityEngine;

public class GenericManager : MonoBehaviour
{
    public void ReturnToMenu()
    {
        AudioManager.singleton.PlayStandardSound();
        GlobalManager.singleton.LoadScene("MenuScene");
    }
}
