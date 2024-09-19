using Mirror;
using UnityEngine;

public class FightUI : MonoBehaviour
{
    [SerializeField] private FightManager manager;

    [SerializeField] private PlayerFightUI playerTop;
    [SerializeField] private PlayerFightUI playerBottom;
    [SerializeField] private CardSlot cardSlot;

    private void Start()
    {
        manager.OnSetupComplete += SetupPlayers;
        manager.OnTurnChanged += ChangePlayers;
        manager.OnMoveReceive += MakeMove;
    }

    private void SetupPlayers(int playerTurn)
    {
        if (NetworkClient.activeHost)
        {
            playerTop.SetupUI(manager.players[1], playerTurn == 1, GlobalManager.singleton.mode == GameMode.Offline);
            playerBottom.SetupUI(manager.players[0], playerTurn == 0, true);
            cardSlot.MoveUp(playerTurn == 0);
        }
        else
        {
            playerTop.SetupUI(manager.players[0], playerTurn == 0, false);
            playerBottom.SetupUI(manager.players[1], playerTurn == 1, true);
            cardSlot.MoveUp(playerTurn == 1);
        }
    }

    private void ChangePlayers(int playerTurn)
    {
        playerTop.MakeInteractable(playerTurn == playerTop.player.playerID, GlobalManager.singleton.mode == GameMode.Offline);
        playerBottom.MakeInteractable(playerTurn == playerBottom.player.playerID, true);
        cardSlot.MoveUp(playerTurn == playerBottom.player.playerID, playerTurn == playerTop.player.playerID || playerTurn == playerBottom.player.playerID);
    }

    private void MakeMove(MoveMessage message)
    {
        if (message.playerIndex == playerTop.player.playerID && GlobalManager.singleton.mode == GameMode.Online)
        {
            playerTop.MakeMove(message);
        }
        else
        {
            manager.timeToMakeMove = 0f;

            cardSlot.PlayAnimation(message.playerIndex < 0, (message.playCard || message.playerIndex < 0) && message.cardPlayed);
        }
    }

    public void GiveUp(PlayerFightUI playerUI)
    {
        AudioManager.singleton.PlayStandardSound();
        manager.SendMove(playerUI.player.playerID);
    }

    private void OnDestroy()
    {
        manager.OnSetupComplete -= SetupPlayers;
        manager.OnTurnChanged -= ChangePlayers;
        manager.OnMoveReceive -= MakeMove;
    }
}
