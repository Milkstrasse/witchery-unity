using Mirror;
using UnityEngine;
using UnityEngine.Localization.Components;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameOverManager manager;
    [SerializeField] private LocalizeStringEvent topText;
    [SerializeField] private LocalizeStringEvent bottomText;

    private void Awake()
    {
        manager.OnSetupComplete += SetupUI;
    }

    private void SetupUI(int player)
    {
        if (NetworkClient.activeHost)
        {
            topText.StringReference.SetReference("StringTable", player == 1 ? "victory" : "defeat");
            bottomText.StringReference.SetReference("StringTable", player == 0 ? "victory" : "defeat");
        }
        else
        {
            topText.StringReference.SetReference("StringTable", player == 0 ? "victory" : "defeat");
            bottomText.StringReference.SetReference("StringTable", player == 1 ? "victory" : "defeat");
        }
    }

    public void ReturnToMenu()
    {
        GlobalManager.singleton.maxPlayers = 0;
        manager.ReturnToMenu();
    }

    private void OnDestroy()
    {
        manager.OnSetupComplete -= SetupUI;
    }
}
