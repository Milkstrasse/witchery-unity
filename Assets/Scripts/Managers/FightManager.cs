using System.Collections;
using Mirror;
using UnityEngine;

public class FightManager : NetworkBehaviour
{
    public Player[] players;

    private GameObject canvas;
    private CardSlot slot;
    private FightUI fightUI;
    private bool acceptMessage;

    private FightLogic logic;

    private void Start()
    {
        logic = new FightLogic();

        canvas = GameObject.Find("Canvas");
        fightUI = canvas.GetComponent<FightUI>();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Player[2]{playerObjects[0].GetComponent<Player>(), playerObjects[1].GetComponent<Player>()};

        SpawnSlot();

        fightUI.enabled = true;
        fightUI.Setup(this);

        acceptMessage = true;

        Invoke("StartFight", 0.3f); //give clients time to subscribe to callbacks
    }

    [ServerCallback]
    private void SpawnSlot()
    {
        GameObject cardSlot = Instantiate(NetworkManager.singleton.spawnPrefabs[1]);
        slot = cardSlot.GetComponent<CardSlot>();

        NetworkServer.Spawn(cardSlot);
    }

    [ServerCallback]
    private void StartFight()
    {
        NetworkServer.RegisterHandler<MoveMessage>(OnReceiveMessage);

        logic.playerTurn = Random.Range(0, 2);

        TurnMessage msg = new TurnMessage {
            turn = logic.playerTurn
        };
        
        NetworkServer.SendToAll(msg);
    }


    private void OnReceiveMessage(NetworkConnectionToClient connection, MoveMessage message)
    {
        if (!acceptMessage)
            return;
        
        Debug.Log("Made Move");

        acceptMessage = false;

        if (message.cardPlayed)
        {
            if (players[logic.playerTurn].energy >= players[logic.playerTurn].cardHand[message.cardIndex].cost)
            {
                foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
                {
                    if (conn != connection)
                    {
                        RpcMakeMove(conn, logic.playerTurn, message.cardIndex);
                    }
                }
            }
        }

        StartCoroutine(MakeMove(message));
    }

    IEnumerator MakeMove(MoveMessage message)
    {
        Card card = players[logic.playerTurn].cardHand[message.cardIndex];

        if (message.cardPlayed)
        {
            if (players[logic.playerTurn].energy >= card.cost)
            {
                players[logic.playerTurn].energy -= card.cost;
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                yield return null;
                
                message.cardPlayed = false;
                message.toRemove = false;
            }
        }
        else
        {
            yield return null;
        }

        if (message.toRemove)
        {
            players[logic.playerTurn].playedCards.Add(card);
            players[logic.playerTurn].cardHand.RemoveAt(message.cardIndex);

            if (message.cardPlayed)
            {
                logic.PlayCard(card, players);
                logic.lastCard = new PlayedCard(logic.playerTurn, card);
                slot.RpcUpdateCard(logic.playerTurn, card);
            }
        }
        else
        {
            slot.RpcUpdateCard(logic.lastCard.playerIndex, logic.lastCard.card);
            players[logic.playerTurn].RpcOnCardsUpdated();
        }

        if (logic.IsGameOver(players))
        {
            NetworkManager.singleton.ServerChangeScene("GameOverScene");
            yield break;
        }
        else
        {
            if ((message.cardPlayed && players[1 - logic.playerTurn].cardHand.Count > 0) || players[logic.playerTurn].cardHand.Count == 0)
            {
                logic.playerTurn = 1 - logic.playerTurn;

                if (players[0].cardHand.Count == 0 && players[1].cardHand.Count == 0)
                {
                    players[logic.playerTurn].NewRound();
                    if (logic.IsGameOver(players))
                    {
                        NetworkManager.singleton.ServerChangeScene("GameOverScene");
                        yield break;
                    }

                    players[1 - logic.playerTurn].NewRound();
                    if (logic.IsGameOver(players))
                    {
                        NetworkManager.singleton.ServerChangeScene("GameOverScene");
                        yield break;
                    }
                }
            }

            TurnMessage msg = new TurnMessage
            {
                turn = logic.playerTurn
            };

            NetworkServer.SendToAll(msg);
            acceptMessage = true;
        }
    }

    [TargetRpc]
    private void RpcMakeMove(NetworkConnectionToClient target, int playerIndex, int cardIndex)
    {
        fightUI.MakeMove(playerIndex, cardIndex);
    }
}