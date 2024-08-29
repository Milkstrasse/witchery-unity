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

        players[playerTurn].energy -= card.move.cost;

        int[] targets = new int[]{playerTurn, 1 - playerTurn};
        for (int i = 0; i < targets.Length; i++)
        {
            players[targets[i]].health += card.move.health[i];
            players[targets[i]].energy += card.move.energy[i];

            if (card.move.effects[i].duration > 0 && players[targets[i]].effects.Count < 5)
            {
                players[targets[i]].effects.Add(card.move.effects[i]);
            }
        }

        RemoveCard(message);

        return true;
    }

    public void RemoveCard(MoveMessage message)
    {
        players[playerTurn].RemoveCard(message.cardIndex);
        NextTurn(message.playCard);
    }

    private void NextTurn(bool cardPlayed)
    {
        if (cardPlayed && players[1 - playerTurn].cardHand.Count > 0)
        {
            playerTurn = 1 - playerTurn;
        }
        else if (players[playerTurn].cardHand.Count == 0)
        {
            playerTurn = 1 - playerTurn;

            if (players[playerTurn].cardHand.Count == 0)
            {
                NewRound();
            }
        }
    }

    private void NewRound()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].energy += 7;
            players[i].FillHand(5);
        }
    }
}
