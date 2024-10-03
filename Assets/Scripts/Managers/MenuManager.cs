using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent stringEvent;

    private void Start()
    {
        GlobalManager.QuitAnyConnection();
        GlobalManager.singleton.joincode = "";

        stringEvent.GetComponentInParent<Button>().interactable = GlobalManager.singleton.isConnected;
    }

    public void SetJoincode(string joincode)
    {
        if (joincode == null || joincode == "")
        {
            GlobalManager.singleton.joincode = "";
            stringEvent.StringReference.SetReference("StringTable", "host");
        }
        else
        {
            GlobalManager.singleton.joincode = joincode;
            stringEvent.StringReference.SetReference("StringTable", "join");
        }
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