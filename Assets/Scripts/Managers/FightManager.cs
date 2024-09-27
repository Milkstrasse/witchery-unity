using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static FightManager singleton;

    public Player[] players;
    private FightLogic logic;

    public event Action<int> OnSetupComplete;
    public event Action<int> OnTurnChanged;
    public event Action<MoveMessage> OnMoveReceive;
    public event Action OnMoveFailed;

    private bool sendingMessage;
    public float timeToMakeMove;

    private CustomQueue messages;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        players = new Player[2];
        logic = new FightLogic();

        messages = new CustomQueue();

        sendingMessage = true;

        NetworkClient.ReplaceHandler<TurnMessage>(OnTurnStart);
        NetworkClient.ReplaceHandler<MoveMessage>(OnMoveReceived);
        NetworkServer.ReplaceHandler<MoveMessage>(OnMoveMade);
    }

    [Server]
    public PlayerMessage SetupPlayer(PlayerMessage message)
    {
        PlayerMessage msg = new PlayerMessage(message.name, message.fighterIDs, message.effects);

        PlayerData playerData = new PlayerData(message);
        logic.players.Add(playerData);

        msg.cardHand = playerData.cardHand.ToArray();

        return msg;
    }

    [Server]
    private void OnMoveMade(NetworkConnectionToClient conn, MoveMessage message)
    {
        //Set effects as not new
        logic.players[0].UnmarkEffects();
        logic.players[1].UnmarkEffects();

        if (conn.connectionId != NetworkClient.connection.connectionId && message.playerIndex != 1)
        {
            conn.Send(new MoveMessage(-1, 0, false));
            conn.Send(new TurnMessage(message.playerIndex, new PlayerData[] { logic.players[message.playerIndex] }, true));
            
            return;
        }

        if (message.cardIndex < 0) //player gave up, winner - 2
        {
            NetworkServer.SendToAll(new TurnMessage(-1 - message.playerIndex, new PlayerData[]{logic.players[message.playerIndex]}));
            return;
        }

        float sendDelay = 0f;
        if (logic.PlayLastCard(message))
        {
            NetworkServer.SendToAll(new MoveMessage(-1, 0, true, true));
            NetworkServer.SendToAll(new TurnMessage(logic.playerTurn - 5, logic.players.ToArray()));
            
            sendDelay = 1.5f;
        }

        StartCoroutine(MakeMove(conn, message, sendDelay));
    }

    IEnumerator MakeMove(NetworkConnectionToClient conn, MoveMessage message, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (logic.MakeMove(message))
        {
            message.cardPlayed = logic.lastCard.played;
            NetworkServer.SendToAll(message);
            NetworkServer.SendToAll(new TurnMessage(logic.playerTurn, logic.players.ToArray()));
        }
        else //no card was played or removed
        {
            conn.Send(new MoveMessage(-1, 0, false));
            conn.Send(new TurnMessage(logic.playerTurn, new PlayerData[] { logic.players[logic.playerTurn] }, true));
        }
    }

    [Client]
    private void OnTurnStart(TurnMessage message)
    {
        if (message.playerTurn >= 0)
        {
            sendingMessage = false;

            if (logic.playerTurn == -1) //start fight
            {
                logic.playerTurn = message.playerTurn;

                GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
                players[0] = playerObjects[0].GetComponent<Player>();
                players[1] = playerObjects[1].GetComponent<Player>();

                OnSetupComplete?.Invoke(logic.playerTurn);
            }
            else
            {
                if (messages.AddToQueue(message, false))
                {
                    StartCoroutine(InvokeQueue());
                }
            }
        }
        else if (message.playerTurn < -2) //last card is being played
        {
            if (messages.AddToQueue(message, false))
            {
                StartCoroutine(InvokeQueue());
            }
        }
        else //Game Over
        {
            OnTurnChanged?.Invoke(-1);

            for (int i = 0; i < message.players.Length; i++)
            {
                PlayerData player = message.players[i];
                players[i].UpdatePlayer(player, true);
            }

            StartCoroutine(EndFight(message.playerTurn + 2));
        }
    }

    IEnumerator EndFight(int winner)
    {
        while (timeToMakeMove > 0f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        players[winner].hasWon = true;
        GlobalManager.singleton.LoadScene("GameOverScene");
    }

    [Client]
    public void SendMove(int cardIndex, bool playCard, bool remove = true)
    {
        sendingMessage = true;

        if (remove)
        {
            players[logic.playerTurn].cardHand.RemoveAt(cardIndex);
            players[logic.playerTurn].OnPlayerChanged?.Invoke();
        }
        
        NetworkClient.Send(new MoveMessage(logic.playerTurn, cardIndex, playCard));
    }

    [Client]
    public void SendMove(int playerIndex)
    {
        sendingMessage = true;
        
        NetworkClient.Send(new MoveMessage(playerIndex, -1, false));
    }

    [Client]
    private void OnMoveReceived(MoveMessage message)
    {
        if (messages.AddToQueue(message, true))
        {
            StartCoroutine(InvokeQueue());
        }
    }

    IEnumerator InvokeQueue()
    {
        while (messages.GetLength() > 0)
        {
            NetworkMessage queueMessage = messages.PopFromQueue();

            if (queueMessage is TurnMessage message)
            {
                logic.playerTurn = message.playerTurn;
                if (logic.playerTurn < 0)
                {
                    logic.playerTurn += 5;
                }

                if (message.players.Length > 1)
                {
                    for (int i = 0; i < message.players.Length; i++)
                    {
                        PlayerData player = message.players[i];
                        players[i].UpdatePlayer(player, message.playerTurn >= 0);
                    }
                }
                else
                {
                    PlayerData player = message.players[0];
                    players[logic.playerTurn].UpdatePlayer(player, true);
                }

                if (message.failed)
                {
                    OnMoveFailed?.Invoke();
                }

                OnTurnChanged?.Invoke(message.playerTurn);
            }
            else
            {
                MoveMessage moveMessage = (MoveMessage)queueMessage;

                OnMoveReceive?.Invoke(moveMessage);
            }

            yield return new WaitForSeconds(0.1f);

            while (timeToMakeMove > 0f)
            {
                yield return null;
            }
        }
    }

    public bool IsAbleToMessage()
    {
        return !sendingMessage;
    }

    public MoveMessage GetMove()
    {
        return CPULogic.GetMove(players[1], logic);
    }

    public void EndFight()
    {
        GlobalManager.singleton.LoadScene("GameOverScene");
    }
}
