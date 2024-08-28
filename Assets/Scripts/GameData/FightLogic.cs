using System.Collections.Generic;

public class FightLogic
{
    public int playerTurn;
    public List<PlayerData> players;

    public FightLogic()
    {
        playerTurn = -1;
        players = new List<PlayerData>();
    }

    public void MakeMove(MoveMessage message)
    {
        players[playerTurn].playedCards.Add(players[playerTurn].cardHand[message.cardIndex]);
        players[playerTurn].cardHand.RemoveAt(message.cardIndex);

        players[1 - playerTurn].health -= 10;
        players[playerTurn].energy -= 2;
    }
}
