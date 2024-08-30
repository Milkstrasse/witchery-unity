using System;
using System.Collections.Generic;

public class FightLogic
{
    public int playerTurn;
    public List<PlayerData> players;
    public int winner;

    public FightLogic()
    {
        playerTurn = -1;
        winner = -1;
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
            players[targets[i]].health = Math.Clamp(players[targets[i]].health + card.move.health[i], 0, 50);
            players[targets[i]].energy += card.move.energy[i];

            if (card.move.effects[i].duration > 0 && players[targets[i]].effects.Count < 5)
            {
                players[targets[i]].effects.Add(card.move.effects[i]);
            }

            if (winner < 0 && players[targets[i]].health == 0)
            {
                winner = targets[1 - i];
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
        if (winner >= 0)
        {
            playerTurn = winner - 2;
            return;
        }

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

                if (winner >= 0)
                {
                    playerTurn = winner - 2;
                    return;
                }
            }
        }
    }

    private void NewRound()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].energy += 7;
            players[i].FillHand(5);

            int j = 0;
            while (j < players[i].effects.Count)
            {
                players[i].effects[j].TriggerEffect(players[i]);

                players[i].effects[j].duration--;
                if (players[i].effects[j].duration <= 0)
                {
                    players[i].effects.RemoveAt(j);
                }
                else
                {
                    j++;
                }

                if (winner < 0 && players[i].health == 0)
                {
                    winner = 1 - i;
                }
            }
        }
    }
}
