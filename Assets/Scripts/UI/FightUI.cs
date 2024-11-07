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
        playerTop.MakeInteractable(playerTurn == playerTop.player.playerID, GlobalManager.singleton.mode == GameMode.Offline, cardSlot);
        playerBottom.MakeInteractable(playerTurn == playerBottom.player.playerID, true, cardSlot);
        cardSlot.MoveUp(playerTurn == playerBottom.player.playerID, playerTurn == playerTop.player.playerID || playerTurn == playerBottom.player.playerID);
    }

    private void MakeMove(MoveMessage message)
    {
        if (message.playerIndex == playerTop.player.playerID && GlobalManager.singleton.mode == GameMode.Online)
        {
            playerTop.MakeMove(message);
        }
        else if (GlobalManager.singleton.mode == GameMode.Online)
        {
            manager.timeToMakeMove = 0f;

            cardSlot.PlayAnimation(message.playerIndex + 5 == playerBottom.player.playerID, message.playCard && message.cardPlayed);
        }
        else
        {
            manager.timeToMakeMove = 0f;

            cardSlot.PlayAnimation(message.playerIndex < 0, message.playCard && message.cardPlayed);
        }
    }

    public void GiveUp(PlayerFightUI playerUI)
    {
        AudioManager.singleton.PlayStandardSound();
        manager.SendMove(playerUI.player.playerID);

        if (playerUI.IsActive())
        {
            cardSlot.MoveUp(false, false);
            playerUI.MakeInteractable(false, true);
        }
        else if (playerUI.player.playerID == playerBottom.player.playerID)
        {
            cardSlot.MoveUp(true, false);
            playerTop.MakeInteractable(false, true);
        }
        else
        {
            cardSlot.MoveUp(false, false);
            playerBottom.MakeInteractable(false, true);
        }
    }

    private void OnDestroy()
    {
        manager.OnSetupComplete -= SetupPlayers;
        manager.OnTurnChanged -= ChangePlayers;
        manager.OnMoveReceive -= MakeMove;
    }
}
