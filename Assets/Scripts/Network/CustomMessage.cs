using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public struct PlayerMessage : NetworkMessage
{
    public string name;
    public int health;
    public int energy;
    public int[] fighterIDs;
    public int[] cardHand;
    public StatusEffect[] effects;

    public PlayerMessage(string name, int[] fighterIDs)
    {
        this.name = name;
        this.fighterIDs = fighterIDs;
        health = 0;
        energy = 0;
        cardHand = new int[0];
        effects = new StatusEffect[0];
    }

    public PlayerMessage(PlayerData playerData)
    {
        name = playerData.name;
        health = playerData.health;
        energy = playerData.energy;
        fighterIDs = new int[0];
        cardHand = playerData.cardHand.ToArray();
        effects = new StatusEffect[0];
    }
}

public struct TurnMessage : NetworkMessage
{
    public int playerTurn;
    public bool failed;

    public TurnMessage(int playerTurn, bool failed = false)
    {
        this.playerTurn = playerTurn;
        this.failed = failed;
    }
}

public struct MoveMessage : NetworkMessage
{
    public int playerIndex;
    public int cardIndex;
    public bool playCard;

    public MoveMessage(int playerIndex, int cardIndex, bool playCard)
    {
        this.playerIndex = playerIndex;
        this.cardIndex = cardIndex;
        this.playCard = playCard;
    }
}