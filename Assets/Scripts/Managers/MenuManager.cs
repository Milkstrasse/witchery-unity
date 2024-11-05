using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        GlobalManager.QuitAnyConnection();
        GlobalManager.singleton.joincode = "";
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

    public void DeletePlayer()
    {
        SaveManager.DeleteData();
    }
}