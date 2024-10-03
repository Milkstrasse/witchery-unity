using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    
    [SerializeField] private GameObject playerPopup;
    [SerializeField] private Button continueButton;

    public void ShowUI()
    {
        playerPopup.SetActive(true);
    }

    public void ChangeName(string name)
    {
        GlobalSettings.playerName = name.Trim();
        continueButton.interactable = GlobalSettings.playerName.Length > 0;
    }
}
