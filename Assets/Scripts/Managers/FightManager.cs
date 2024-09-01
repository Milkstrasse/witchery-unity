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
    public bool makingAMove;

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
        if (message.cardIndex < 0) //player gave up, winner - 2
        {
            NetworkServer.SendToAll(new TurnMessage(-1 - message.playerIndex, new PlayerData[]{logic.players[message.playerIndex]}));
            return;
        }

        if (!message.playCard)
        {
            logic.RemoveCard(message);

            if (GlobalManager.singleton.maxPlayers > 1)
            {
                NetworkServer.SendToAll(message);
            }

            NetworkServer.SendToAll(new TurnMessage(logic.playerTurn, logic.players.ToArray()));
        }
        else if (logic.MakeMove(message))
        {
           if (GlobalManager.singleton.maxPlayers > 1)
            {
                NetworkServer.SendToAll(message);
            }
            
            NetworkServer.SendToAll(new TurnMessage(logic.playerTurn, logic.players.ToArray()));
        }
        else
        {
            NetworkServer.SendToAll(new TurnMessage(logic.playerTurn, new PlayerData[]{logic.players[logic.playerTurn]}, true));
        }
    }

    [Client]
    private void OnTurnStart(TurnMessage message)
    {
        sendingMessage = false;

        if (message.playerTurn >= 0)
        {
            if (logic.playerTurn < 0)
            {
                logic.playerTurn = message.playerTurn;

                GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
                players[0] = playerObjects[0].GetComponent<Player>();
                players[1] = playerObjects[1].GetComponent<Player>();

                OnSetupComplete?.Invoke(logic.playerTurn);
            }
            else
            {
                logic.playerTurn = message.playerTurn;

                if (message.players.Length > 1)
                {
                    for (int i = 0; i < message.players.Length; i++)
                    {
                        PlayerData player = message.players[i];
                        bool updateCards = (NetworkClient.activeHost && i == 0) || (!NetworkClient.activeHost && i == 1);
                        updateCards = updateCards || player.cardHand.Count >= players[i].cardHand.Count;
                        players[i].UpdatePlayer(player, updateCards);
                    }
                }
                else
                {
                    PlayerData player = message.players[0];
                    bool updateCards = player.cardHand.Count >= players[message.playerTurn].cardHand.Count || GlobalManager.singleton.maxPlayers < 2;
                    players[message.playerTurn].UpdatePlayer(player, updateCards);
                }

                bool isActivePlayer = (NetworkClient.activeHost && message.playerTurn == 0) || (!NetworkClient.activeHost && message.playerTurn == 1);
                if (message.failed && (isActivePlayer || GlobalManager.singleton.maxPlayers < 2))
                {
                    OnMoveFailed?.Invoke();
                }

                OnTurnChanged?.Invoke(logic.playerTurn);
            }

            Debug.Log(logic.playerTurn);
        }
        else //Game Over
        {
            OnTurnChanged?.Invoke(-1);

            for (int i = 0; i < message.players.Length; i++)
            {
                PlayerData player = message.players[i];
                bool updateCards = (NetworkClient.activeHost && i == 0) || (!NetworkClient.activeHost && i == 1);
                updateCards = updateCards || player.cardHand.Count >= players[i].cardHand.Count;
                players[i].UpdatePlayer(player, updateCards);
            }

            StartCoroutine(EndFight(message.playerTurn + 2));
        }
    }

    IEnumerator EndFight(int winner)
    {
        while (makingAMove)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.5f);

        players[winner].hasWon = true;
        GlobalManager.singleton.LoadScene("GameOverScene");
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
    public void SendMove(int playerIndex)
    {
        sendingMessage = true;
        
        NetworkClient.Send(new MoveMessage(playerIndex, -1, false));
    }

    [Client]
    private void OnMoveReceived(MoveMessage message)
    {
        makingAMove = true;
        OnMoveReceive?.Invoke(message);
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
