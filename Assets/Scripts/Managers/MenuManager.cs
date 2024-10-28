using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform mainMenu;
    [SerializeField] private RectTransform modeMenu;

    [SerializeField] private Button joinButton;
    [SerializeField] private Button hostButton;

    private void Start()
    {
        GlobalManager.QuitAnyConnection();
        GlobalManager.singleton.joincode = "";

        joinButton.interactable = GlobalManager.singleton.isConnected;
        hostButton.interactable = GlobalManager.singleton.isConnected;

        if (GlobalManager.singleton.maxPlayers > 0)
        {
            mainMenu.localPosition = new Vector3(-mainMenu.sizeDelta.x * 1.5f - 20f, mainMenu.localPosition.y, mainMenu.localPosition.z);
            modeMenu.localPosition = new Vector3(-mainMenu.sizeDelta.x * 0.5f, modeMenu.localPosition.y, modeMenu.localPosition.z);
        }
    }

    public void SwitchToModeMenu()
    {
        AudioManager.singleton.PlayStandardSound();

        GlobalManager.singleton.maxPlayers = 5;

        LeanTween.moveLocalX(mainMenu.gameObject, -mainMenu.sizeDelta.x * 1.5f - 20f, 0.3f);
        LeanTween.moveLocalX(modeMenu.gameObject, -mainMenu.sizeDelta.x * 0.5f, 0.3f);
    }

    public void SwitchToMainMenu()
    {
        AudioManager.singleton.PlayStandardSound();
        
        GlobalManager.singleton.maxPlayers = 0;

        LeanTween.moveLocalX(mainMenu.gameObject, -mainMenu.sizeDelta.x * 0.5f, 0.3f);
        LeanTween.moveLocalX(modeMenu.gameObject, mainMenu.sizeDelta.x * 0.5f + 20f, 0.3f);
    }

    public void SetJoincode(string joincode)
    {
        GlobalManager.singleton.joincode = joincode;
    }

    public void StartSelection()
    {
        if (GlobalManager.singleton.joincode == "")
            return;
        
        AudioManager.singleton.PlayStandardSound();

        GlobalManager.singleton.mode = GameMode.Online;
        
        GlobalManager.singleton.maxPlayers = 2;
        GlobalManager.singleton.LoadScene("SelectionScene");
    }

    public void StartSelection(int mode)
    {
        AudioManager.singleton.PlayStandardSound();
        
        GlobalManager.singleton.mode = (GameMode) mode;
        if (mode == 0)
        {
            GlobalManager.singleton.maxPlayers = 2;
        }
        else
        {
            GlobalManager.singleton.maxPlayers = 1;
        }
        
        GlobalManager.singleton.LoadScene("SelectionScene");
    }

    public void GoToScene(string scene)
    {
        AudioManager.singleton.PlayStandardSound();
        GlobalManager.singleton.LoadScene(scene);
    }
}