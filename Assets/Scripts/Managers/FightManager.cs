using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static FightManager singleton;

    public Player[] players;
    private FightLogic logic;

    public event Action OnSetupComplete;
    public event Action<MoveMessage> OnMoveReceive;
    public event Action OnMoveFailed;

    private bool sendingMessage;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        players = new Player[2];
        logic = new FightLogic();

        sendingMessage = true;

        NetworkClient.ReplaceHandler<TurnMessage>(OnTurnStart);
        NetworkClient.ReplaceHandler<MoveMessage>(OnMoveReceived);
        NetworkServer.ReplaceHandler<MoveMessage>(OnMoveMade);
    }

    [Server]
    public PlayerMessage SetupPlayer(PlayerMessage message)
    {
        List<int> cards = new List<int>();

        PlayerMessage msg = new PlayerMessage(message.name, message.fighterIDs);
        for (int i = 0; i < message.fighterIDs.Length; i++)
        {
            Fighter fighter = GlobalManager.singleton.fighters[message.fighterIDs[i]];
            for (int j = 0; j < fighter.moves.Length; j++)
            {
                cards.Add(i * fighter.moves.Length + j);
            }
        }

        PlayerData playerData = new PlayerData(message.name, cards);
        logic.players.Add(playerData);

        msg.cardHand = playerData.cardHand.ToArray();

        return msg;
    }

    [Server]
    private void OnMoveMade(MoveMessage message)
    {
        if (!message.playCard)
        {
            logic.RemoveCard(message);

            NetworkServer.SendToAll(message);

            NetworkServer.SendToAll(new PlayerMessage(logic.players[1 - logic.playerTurn]));
            NetworkServer.SendToAll(new PlayerMessage(logic.players[logic.playerTurn]));

            NetworkServer.SendToAll(new TurnMessage(logic.playerTurn));

            return;
        }

        if (logic.MakeMove(message))
        {
            NetworkServer.SendToAll(message);
            NetworkServer.SendToAll(new PlayerMessage(logic.players[1 - logic.playerTurn]));
            NetworkServer.SendToAll(new PlayerMessage(logic.players[logic.playerTurn]));

            NetworkServer.SendToAll(new TurnMessage(logic.playerTurn));
        }
        else
        {
            NetworkServer.SendToAll(new PlayerMessage(logic.players[logic.playerTurn]));
            NetworkServer.SendToAll(new TurnMessage(logic.playerTurn, true));
        }
    }

    [Client]
    private void OnTurnStart(TurnMessage message)
    {
        sendingMessage = false;

        if (logic.playerTurn < 0)
        {
            NetworkClient.ReplaceHandler<PlayerMessage>(OnReceivePlayer);

            logic.playerTurn = message.playerTurn;

            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            players[0] = playerObjects[0].GetComponent<Player>();
            players[1] = playerObjects[1].GetComponent<Player>();

            OnSetupComplete?.Invoke();
        }
        else
        {
            logic.playerTurn = message.playerTurn;

            bool isActivePlayer = (NetworkClient.activeHost && message.playerTurn == 0) || (!NetworkClient.activeHost && message.playerTurn == 1);
            if (message.failed && isActivePlayer)
            {
                OnMoveFailed?.Invoke();
            }
        }

        Debug.Log(logic.playerTurn);
    }

    [Client]
    public void SendMove(int cardIndex, bool playCard)
    {
        sendingMessage = true;

        players[logic.playerTurn].cardHand.RemoveAt(cardIndex);
        players[logic.playerTurn].OnPlayerChanged?.Invoke();
        
        NetworkClient.Send(new MoveMessage(logic.playerTurn, cardIndex, playCard));
    }

    [Client]
    private void OnMoveReceived(MoveMessage message)
    {
        OnMoveReceive?.Invoke(message);
    }

    [Client]
    private void OnReceivePlayer(PlayerMessage message)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (message.name == players[i].playerName)
            {
                bool updateCards = (NetworkClient.activeHost && i == 0) || (!NetworkClient.activeHost && i == 1);
                updateCards = updateCards || message.cardHand.Length >= players[i].cardHand.Count;
                players[i].UpdatePlayer(message, updateCards);
            }
        }
    }

    public bool IsAbleToMessage()
    {
        return !sendingMessage;
    }

    public void EndFight()
    {
        GlobalManager.singleton.LoadScene("GameOverScene");
    }
}
