using UnityEngine;
using UnityEngine.Localization.Components;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent stringEvent;

    private void Start()
    {
        GlobalManager.QuitAnyConnection();
        GlobalManager.singleton.relayEnabled = false;
        GlobalManager.singleton.joincode = "";
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

    public void EnableRelay(bool relayEnabled) => GlobalManager.singleton.relayEnabled = relayEnabled;

    public void StartSelection(int max)
    {
        GlobalManager.singleton.maxPlayers = max;
        GlobalManager.singleton.LoadScene("SelectionScene");
    }
}
