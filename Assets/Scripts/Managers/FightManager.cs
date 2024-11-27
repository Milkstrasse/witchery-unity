using System;
using System.Collections;
using System.Diagnostics;
using Mirror;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static FightManager singleton;

    public PlayerObject[] players;
    private FightLogic logic;

    public event Action<int> OnSetupComplete;
    public event Action<int> OnTurnChanged;
    public event Action<MoveMessage> OnMoveReceive;
    public event Action OnMoveFailed;

    private bool sendingMessage;
    public float timeToMakeMove;

    private MessageQueue messages;

    private Stopwatch stopwatch;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        stopwatch = new Stopwatch();

        players = new PlayerObject[2];
        logic = new FightLogic();

        messages = new MessageQueue();

        sendingMessage = true;

        NetworkClient.ReplaceHandler<TurnMessage>(OnTurnStart);
        NetworkClient.ReplaceHandler<MoveMessage>(OnMoveReceived);
        NetworkServer.ReplaceHandler<MoveMessage>(OnMoveMade);
    }

    [Server]
    public PlayerMessage SetupPlayer(PlayerMessage message)
    {
        PlayerMessage msg = new PlayerMessage(message.name, message.fighterIDs);

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

        if (!logic.players[0].startedFirst && !logic.players[1].startedFirst)
        {
            logic.players[logic.playerTurn].startedFirst = true;
        }

        if (message.cardIndex < 0) //player gave up, winner - 2
        {
            NetworkServer.SendToAll(new TurnMessage(-1 - message.playerIndex, new PlayerData[]{logic.players[message.playerIndex]}));
            return;
        }

        float sendDelay = 0f;
        if (logic.PlayLastCard(message))
        {
            NetworkServer.SendToAll(new MoveMessage(message.playCard ? logic.playerTurn - 5 : logic.playerTurn + 5, 0, true, true));
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

            if (logic.playerTurn == -1) //start game
            {
                stopwatch.Start();

                logic.playerTurn = message.playerTurn;

                GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
                players[0] = playerObjects[0].GetComponent<PlayerObject>();
                players[1] = playerObjects[1].GetComponent<PlayerObject>();

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
            stopwatch.Stop();

            players[message.playerTurn + 2].hasWon = true;

            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds/10:00}";

            UnityEngine.Debug.Log("Fight ended after " + elapsedTime);
            UnityEngine.Debug.Log($"{logic.players[0].roundsPlayed} round(s) were played");
            UnityEngine.Debug.Log($"First player has won ? {(logic.players[0].startedFirst && players[0].hasWon) || (logic.players[1].startedFirst && players[1].hasWon)}");

            if (messages.AddToQueue(message, false))
            {
                StartCoroutine(InvokeQueue());
            }

            StartCoroutine("EndFight");
        }
    }

    IEnumerator EndFight()
    {
        while (timeToMakeMove > 0f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

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

                for (int i = 0; i < message.players.Length; i++)
                {
                    PlayerData player = message.players[i];
                    players[i].UpdatePlayer(player, message.playerTurn >= 0);

                    if ((NetworkServer.activeHost && i == 0) || (!NetworkServer.activeHost && i == 1))
                    {
                        SaveManager.UpdateStats(player, players[0].hasWon || players[1].hasWon, players[i].hasWon);
                    }
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
}
