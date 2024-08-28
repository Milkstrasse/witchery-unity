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

    public bool MakeMove(MoveMessage message)
    {
        int cardIndex = players[playerTurn].cardHand[message.cardIndex];
        Card card = FightManager.singleton.players[playerTurn].cards[cardIndex];

        if (card.move.cost > players[playerTurn].energy)
        {
            return false;
        }

        players[playerTurn].playedCards.Add(cardIndex);
        players[playerTurn].cardHand.RemoveAt(message.cardIndex);

        players[1 - playerTurn].health -= 10;
        players[playerTurn].energy -= card.move.cost;

        return true;
    }
}
