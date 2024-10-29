using Mirror;
using UnityEngine;
using UnityEngine.Localization.Components;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameOverManager manager;

    [SerializeField] private PlayerOverUI topPlayer;
    [SerializeField] private PlayerOverUI bottomPlayer;

    [SerializeField] private LocalizeStringEvent topText;
    [SerializeField] private LocalizeStringEvent bottomText;

    private void Awake()
    {
        manager.OnSetupComplete += SetupUI;
    }

    private void SetupUI(Player[] players)
    {
        if (NetworkClient.activeHost)
        {
            topPlayer.UpdateUI(players[1]);
            bottomPlayer.UpdateUI(players[0]);
        }
        else
        {
            topPlayer.UpdateUI(players[0]);
            bottomPlayer.UpdateUI(players[1]);
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
