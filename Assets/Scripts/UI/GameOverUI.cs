using Mirror;
using UnityEngine;
using UnityEngine.Localization.Components;

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
            topPlayer.UpdateUI(players[1], players[0].energy);
            bottomPlayer.UpdateUI(players[0], players[0].energy);
        }
        else
        {
            topPlayer.UpdateUI(players[0], players[1].energy);
            bottomPlayer.UpdateUI(players[1], players[1].energy);
        }
    }

    private void OnDestroy()
    {
        manager.OnSetupComplete -= SetupUI;
    }
}
