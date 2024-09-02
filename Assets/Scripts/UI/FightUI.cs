using Mirror;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
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
            playerTop.SetupUI(manager.players[1], playerTurn == 1);
            playerBottom.SetupUI(manager.players[0], playerTurn == 0);
        }
        else
        {
            playerTop.SetupUI(manager.players[0], playerTurn == 0);
            playerBottom.SetupUI(manager.players[1], playerTurn == 1);
        }
    }

    private void ChangePlayers(int playerTurn)
    {
        playerTop.MakeInteractable(playerTurn == playerTop.player.playerID);
        playerBottom.MakeInteractable(playerTurn == playerBottom.player.playerID);
    }

    private void MakeMove(MoveMessage message)
    {
        if (message.playerIndex == playerTop.player.playerID)
        {
            playerTop.MakeMove(message, cardSlot);
        }
        else
        {
            manager.timeToMakeMove = 0f;
        }
    }

    public void GiveUp(PlayerFightUI playerUI)
    {
        manager.SendMove(playerUI.player.playerID);
    }

    private void OnDestroy()
    {
        manager.OnSetupComplete -= SetupPlayers;
        manager.OnTurnChanged -= ChangePlayers;
        manager.OnMoveReceive -= MakeMove;
    }
}
