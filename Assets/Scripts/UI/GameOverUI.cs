using Mirror;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameOverManager manager;

    [SerializeField] private PlayerOverUI topPlayer;
    [SerializeField] private PlayerOverUI bottomPlayer;

    private void Awake()
    {
        manager.OnSetupComplete += SetupUI;
    }

    private void SetupUI(PlayerObject[] players)
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

    private void OnDestroy()
    {
        manager.OnSetupComplete -= SetupUI;
    }
}
