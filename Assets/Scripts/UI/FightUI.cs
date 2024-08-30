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
        Canvas canvas = GetComponent<Canvas>();

        if (NetworkClient.activeHost)
        {
            playerTop.SetupUI(canvas, manager.players[1], playerTurn == 1);
            playerBottom.SetupUI(canvas, manager.players[0], playerTurn == 0);
        }
        else
        {
            playerTop.SetupUI(canvas, manager.players[0], playerTurn == 0);
            playerBottom.SetupUI(canvas, manager.players[1], playerTurn == 1);
        }
    }

    private void ChangePlayers(int playerTurn)
    {
        if (NetworkClient.activeHost)
        {
            playerTop.MakeInteractable(playerTurn == 1);
            playerBottom.MakeInteractable(playerTurn == 0);
        }
        else
        {
            playerTop.MakeInteractable(playerTurn == 0);
            playerBottom.MakeInteractable(playerTurn == 1);
        }
    }

    private void MakeMove(MoveMessage message)
    {
        if (NetworkClient.activeHost && message.playerIndex == 1)
        {
            playerTop.MakeMove(message, cardSlot);
        }
        else if (!NetworkClient.activeHost && message.playerIndex == 0)
        {
            playerTop.MakeMove(message, cardSlot);
        }
        else
        {
            manager.makingAMove = false;
        }
    }

    private void OnDestroy()
    {
        manager.OnSetupComplete -= SetupPlayers;
        manager.OnTurnChanged -= ChangePlayers;
        manager.OnMoveReceive -= MakeMove;
    }
}
