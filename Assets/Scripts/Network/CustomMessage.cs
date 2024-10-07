using System;
using Mirror;

public struct PlayerMessage : NetworkMessage
{
    public string name;
    public int icon;
    public int health;
    public int energy;
    public SelectedFighter[] fighterIDs;
    public int[] cardHand;
    public StatusEffect[] effects;

    public PlayerMessage(string name, int icon, SelectedFighter[] fighterIDs)
    {
        this.name = name;
        this.icon = icon;
        this.fighterIDs = fighterIDs;
        health = 0;
        energy = 0;
        cardHand = new int[0];
        effects = new StatusEffect[0];
    }
}

public struct TurnMessage : NetworkMessage
{
    public int playerTurn;
    public bool failed;
    public PlayerData[] players; 

    public TurnMessage(int playerTurn, PlayerData[] players, bool failed = false)
    {
        this.playerTurn = playerTurn;
        this.failed = failed;
        this.players = players;
    }
}

public struct MoveMessage : NetworkMessage
{
    public int playerIndex;
    public int cardIndex;
    public bool playCard;
    public bool cardPlayed;

    public MoveMessage(int playerIndex, int cardIndex, bool playCard, bool cardPlayed = false)
    {
        this.playerIndex = playerIndex;
        this.cardIndex = cardIndex;
        this.playCard = playCard;

        this.cardPlayed = cardPlayed;
    }
}

[Serializable]
public struct SelectedFighter
{
    public int fighterID;
    public int outfit;

    public SelectedFighter(int fighterID, int outfit)
    {
        this.fighterID = fighterID;
        this.outfit = outfit;
    }
}