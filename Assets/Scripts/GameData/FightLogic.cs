using System;
using System.Collections.Generic;
using UnityEngine;

public class FightLogic
{
    public int playerTurn;
    public List<PlayerData> players;
    public PlayedCard lastCard;
    public int winner;

    public FightLogic()
    {
        playerTurn = -1;
        winner = -1;
        players = new List<PlayerData>();
    }

    public bool MakeMove(MoveMessage message)
    {
        if (!message.playCard)
        {
            RemoveCard(message);
            return true;
        }

        int cardIndex = players[playerTurn].cardHand[message.cardIndex];
        Card card = FightManager.singleton.players[playerTurn].cards[cardIndex];

        if (card.move.cost > players[playerTurn].energy)
        {
            return false;
        }

        players[playerTurn].energy -= card.move.cost;

        bool wasPlayed = PlayCard(card, playerTurn, true);
        lastCard = new PlayedCard(card, playerTurn, wasPlayed);

        RemoveCard(message);

        return true;
    }

    public void RemoveCard(MoveMessage message)
    {
        players[playerTurn].RemoveCard(message.cardIndex);
        NextTurn(message.playCard);
    }

    private bool PlayCard(Card card, int turn, bool blockable)
    {
        switch (card.move.moveType)
        {
            case Move.MoveType.Standard:
                if (blockable && FightManager.singleton.players[1 - turn].HasResponse(card.move))
                {
                    return false;
                }

                int[] targets = new int[] { turn, 1 - turn };
                for (int i = 0; i < targets.Length; i++)
                {
                    int health = card.move.health[i];
                    if (health < 0)
                    {
                        health = Math.Min(health - players[turn].GetPowerBonus(), -1);
                    }
                    else if (health > 0)
                    {
                        health = Math.Max(health + players[turn].GetPowerBonus(), 0);
                    }
                    
                    players[targets[i]].health = Math.Clamp(players[targets[i]].health + health, 0, 50);
                    players[targets[i]].energy += card.move.energy[i];

                    if (card.move.effects[i].duration > 0 && players[targets[i]].effects.Count < 5)
                    {
                        StatusEffect effect = new StatusEffect(card.move.effects[i]);
                        players[targets[i]].effects.Add(effect);
                    }

                    if (winner < 0 && players[targets[i]].health == 0)
                    {
                        winner = targets[1 - i];
                    }
                }

                break;
            default:
                break;
        }

        return true;
    }

    public bool PlayLastCard(MoveMessage message)
    {
        if (lastCard == null || lastCard.played)
            return false;

        int cardIndex = players[message.playerIndex].cardHand[message.cardIndex];
        Card card = FightManager.singleton.players[message.playerIndex].cards[cardIndex];

        if (card.move.moveType == Move.MoveType.Response)
        {
            lastCard.played = true;
            return false;
        }

        PlayCard(lastCard.card, lastCard.player, false);
        lastCard.played = true;

        return true;
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
