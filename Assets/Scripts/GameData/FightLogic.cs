using System;
using System.Collections.Generic;
using UnityEngine;

public class FightLogic
{
    public int playerTurn;
    public List<PlayerData> players;
    public PlayedCard lastCard;
    public int winner;

    private int costBonus;
    private int powerBonus;

    public FightLogic()
    {
        playerTurn = -1;
        winner = -1;
        players = new List<PlayerData>();

        lastCard = new PlayedCard();
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

        if (lastCard.card.hasMove && lastCard.card.move.moveType == Move.MoveType.Combo)
        {
            costBonus = lastCard.card.move.energy[0];
            powerBonus = lastCard.card.move.health[0];
        }
        else
        {
            costBonus = 0;
            powerBonus = 0;
        }

        if (card.hasMove)
        {
            if (card.move.cost - costBonus > players[playerTurn].energy)
            {
                return false;
            }

            players[playerTurn].energy = players[playerTurn].energy - Math.Max(card.move.cost - costBonus, 0);

            if (card.move.moveType == Move.MoveType.Combo)
            {
                message.playCard = false;
            }
        }

        bool wasPlayed = PlayCard(card, playerTurn, true);
        lastCard = new PlayedCard(card, playerTurn, wasPlayed);

        RemoveCard(message);

        return true;
    }

    public void RemoveCard(MoveMessage message)
    {
        players[playerTurn].RemoveCard(message.cardIndex);

        if (lastCard.card.hasMove && lastCard.card.move.moveID == 17)
        {
            players[playerTurn].FillHand(5 - players[playerTurn].cardHand.Count);
        }
        
        NextTurn(message.playCard);
    }

    private bool PlayCard(Card card, int turn, bool blockable)
    {
        if (!card.hasMove)
        {
            return true;
        }
        
        Move move = card.move;
        if (move.moveID == 16 && lastCard != null) //replay last card
        {
            if (!lastCard.card.hasMove)
            {
                return true;
            }

            move = lastCard.card.move;
        }

        if (move.moveType == Move.MoveType.Standard)
        {
            if (blockable && FightManager.singleton.players[1 - turn].HasResponse(move))
            {
                return false;
            }

            switch (move.moveID)
            {
                case 6: //steal energy
                    int tempEnergy = players[1 - turn].energy;
                    players[1 - turn].energy = Math.Max(players[1 - turn].energy + move.energy[1] - players[turn].GetPowerBonus() - powerBonus, 0);
                    tempEnergy -= players[1 - turn].energy;
                    players[turn].energy += tempEnergy;

                    break;
                case 7: //steal health
                    int tempHealth = players[1 - turn].health;
                    players[1 - turn].health = Math.Max(players[1 - turn].health + move.health[1] - players[turn].GetPowerBonus() - powerBonus, 0);
                    tempHealth -= players[1 - turn].energy;
                    players[turn].energy += tempHealth;

                    break;
                case 8: //swap effects
                    List<StatusEffect> temp = players[0].effects;
                    players[0].effects = players[1].effects;
                    players[1].effects = temp;

                    break;
                case 9: //clear effects
                    players[0].effects = new List<StatusEffect>();
                    players[1].effects = new List<StatusEffect>();

                    break;
                case 13: //redistribute HP
                    int allHealth = players[0].health + players[1].health;
                    players[0].health = allHealth / 2;
                    players[1].health = allHealth / 2;

                    break;
                case 14: //add blank
                    players[1 - turn].startIndex = Math.Max(players[1 - turn].startIndex - 1, 0);
                    break;
                case 15: //clear blank
                    players[turn].startIndex = 5;
                    break;
                case 16: //replay last card
                    break;
                case 17: //fill hand
                    break;
                default:
                    int[] targets = new int[] { turn, 1 - turn };
                    for (int i = 0; i < targets.Length; i++)
                    {
                        int health = move.health[i];

                        if (move.moveID == 10 || move.moveID == 11)
                        {
                            health *= lastCard.card.hasMove ? lastCard.card.move.cost : 0;
                        }

                        if (health < 0)
                        {
                            health = Math.Min(health - players[turn].GetPowerBonus() - powerBonus, 0);
                        }
                        else if (health > 0)
                        {
                            health = Math.Max(health + players[turn].GetPowerBonus() + powerBonus, 0);
                        }

                        players[targets[i]].health = Math.Clamp(players[targets[i]].health + health, 0, 50);

                        int energy = move.energy[i];

                        if (move.moveID == 12)
                        {
                            energy *= lastCard.card.hasMove ? lastCard.card.move.cost : 0;
                        }

                        if (energy < 0)
                        {
                            energy = Math.Min(energy - players[turn].GetPowerBonus() - powerBonus, 0);
                        }
                        else if (energy > 0)
                        {
                            energy = Math.Max(energy + players[turn].GetPowerBonus() + powerBonus, 0);
                        }

                        players[targets[i]].energy += energy;

                        if (move.effects[i].duration > 0)
                        {
                            StatusEffect effect = new StatusEffect(move.effects[i]);
                            players[targets[i]].AddEffect(effect);
                        }

                        if (winner < 0 && players[targets[i]].health == 0)
                        {
                            winner = targets[1 - i];
                        }
                    }

                    break;
            }
        }
        else if (move.moveType == Move.MoveType.Response)
        {
            if (lastCard != null && !lastCard.played)
            {
                if (move.moveID == 1)
                {
                    int health = Math.Max(lastCard.card.move.health[0] + players[turn].GetPowerBonus(), 0);
                    players[turn].health = Math.Clamp(players[turn].health + health, 0, 50);
                }
            }
        }

        return true;
    }

    public bool PlayLastCard(MoveMessage message)
    {
        if (lastCard == null || lastCard.played)
            return false;

        int cardIndex = players[message.playerIndex].cardHand[message.cardIndex];
        Card card = FightManager.singleton.players[message.playerIndex].cards[cardIndex];

        if (card.hasMove && card.move.moveType == Move.MoveType.Response)
        {
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
