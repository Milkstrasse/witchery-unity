using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    
    [SerializeField] private GameObject playerPopup;
    [SerializeField] private Button continueButton;

    public void ShowUI()
    {
        playerPopup.SetActive(true);

        SaveManager.savedData = new SavedData();
    }

    public void ChangeName(string name)
    {
        SaveManager.savedData.name = name.Trim().ToUpper();
        continueButton.interactable = SaveManager.savedData.name.Length > 0;
    }
}
