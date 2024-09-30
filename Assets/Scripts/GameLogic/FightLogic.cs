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

        if (card.hasMove)
        {
            if (card.move.cost > players[playerTurn].energy)
            {
                return false;
            }
            else if (card.move.moveType == MoveType.Response && lastCard.played)
            {
                return false;
            }

            players[playerTurn].energy = players[playerTurn].energy - Math.Max(card.move.cost, 0);
        }

        bool wasPlayed = PlayCard(card, playerTurn, true);
        lastCard = new PlayedCard(card, playerTurn, wasPlayed, lastCard);

        RemoveCard(message);

        return true;
    }

    public void RemoveCard(MoveMessage message)
    {
        players[playerTurn].RemoveCard(message.cardIndex);

        if (lastCard.card.hasMove && message.playCard)
        {
            switch (lastCard.card.move.moveID)
            {
                case 1: //fill hand
                    players[playerTurn].FillHand(5 - players[playerTurn].cardHand.Count);
                    break;
                case 7: //remove random card
                    int cardAmount = players[1 - playerTurn].cardHand.Count;
                    if (cardAmount > 0)
                    {
                        players[1 - playerTurn].cardHand.RemoveAt(UnityEngine.Random.Range(0, cardAmount));
                    }

                    break;
                default:
                    break;
            }
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
        if (move.moveID == 5 && lastCard != null) //replay last card
        {
            if (!lastCard.card.hasMove)
            {
                return true;
            }

            move = lastCard.card.move;
        }

        if (move.moveType == MoveType.Standard)
        {
            if (blockable && players[turn].GetEffect("accurate") == null && FightManager.singleton.players[1 - turn].HasResponse(move))
            {
                return false;
            }

            switch (move.moveID)
            {
                case 1: //fill hand
                    break;
                case 2: //redistribute HP
                    int allHealth = players[0].health + players[1].health;
                    players[0].health = allHealth / 2;
                    players[1].health = allHealth / 2;

                    break;
                case 5: //replay last card
                    break;
                case 6: //heal to health
                    players[(move.target + turn)%2].health = Math.Max(players[(move.target + turn)%2].health, move.health + players[turn].GetPowerBonus());
                    break;
                case 10: //swap effects
                    List<StatusEffect> tempEffects = players[0].effects;
                    players[0].effects = players[1].effects;
                    players[1].effects = tempEffects;

                    break;
                case 11: //clear effects
                    if (move.target == 2)
                    {
                        players[0].effects = new List<StatusEffect>();
                        players[1].effects = new List<StatusEffect>();
                    }
                    else
                    {
                        players[(move.target + turn)%2].effects = new List<StatusEffect>();
                    }

                    break;
                case 13: //add blank
                    players[(move.target + turn)%2].startIndex = Math.Max(players[(move.target + turn)%2].startIndex - 1, 0);
                    break;
                case 14: //clear blank
                    players[(move.target + turn)%2].startIndex = 5;
                    break;
                case 20: //steal energy
                    int tempEnergy = players[1 - turn].energy;
                    players[1 - turn].energy = Math.Max(players[1 - turn].energy + move.energy - players[turn].GetPowerBonus(), 0);
                    tempEnergy -= players[1 - turn].energy;
                    players[turn].energy += tempEnergy;

                    break;
                case 28: //steal health
                    int tempHealth = players[1 - turn].health;
                    players[1 - turn].health = Math.Max(players[1 - turn].health + move.health - players[turn].GetPowerBonus(), 0);
                    tempHealth -= players[1 - turn].health;
                    players[turn].health = Math.Clamp(players[turn].health + tempHealth, 0, 50);

                    break;
                default:
                    int health = move.health;
                    if (move.moveID == 15 || move.moveID == 16) //special moves
                    {
                        health *= lastCard.card.hasMove ? lastCard.card.move.cost : 0;
                    }

                    if (health < 0)
                    {
                        health = Math.Min(health - players[turn].GetPowerBonus() + players[(move.target + turn)%2].GetShields(), 0);
                    }
                    else if (health > 0)
                    {
                        health = Math.Max(health + players[turn].GetPowerBonus(), 0);
                    }

                    players[(move.target + turn)%2].health = Math.Clamp(players[(move.target + turn)%2].health + health, 0, 50);

                    if (winner < 0 && players[(move.target + turn)%2].health == 0)
                    {
                        winner = 1 - (move.target + turn)%2;
                    }

                    if (move.health < 0)
                    {
                        StatusEffect spice = players[(move.target + turn)%2].GetEffect("spice");
                        if (spice != null)
                        {
                            players[turn].health = Math.Max(players[turn].health - spice.value, 0);
                            if (winner < 0 && players[turn].health == 0)
                            {
                                winner = 1 - turn;
                            }
                        }
                    }

                    int energy = move.energy;
                    if (move.moveID == 21)
                    {
                        energy *= lastCard.card.hasMove ? lastCard.card.move.cost : 0;
                    }

                    if (energy < 0)
                    {
                        energy = Math.Min(energy - players[turn].GetPowerBonus(), 0);
                    }
                    else if (energy > 0)
                    {
                        energy = Math.Max(energy + players[turn].GetPowerBonus(), 0);
                    }

                    players[(move.target + turn)%2].energy += energy;

                    if (move.effect.duration > 0)
                    {
                        StatusEffect effect = new StatusEffect(move.effect);
                        players[(move.target + turn)%2].AddEffect(effect);
                    }

                    break;
            }
        }
        else if (move.moveType == MoveType.Response)
        {
            if (lastCard.card.hasMove && !lastCard.played)
            {
                if (move.moveID == 3)
                {
                    int health = lastCard.card.move.health;

                    if (health != 0)
                    {
                        if (lastCard.card.move.moveID == 15) //special heal
                        {
                            health *= lastCard.lastCost;
                        }
                        else if (lastCard.card.move.moveID == 6) //heal to health
                        {
                            players[turn].health = Math.Max(players[turn].health, health + players[1 - turn].GetPowerBonus());
                            return true;
                        }

                        health += players[1 - turn].GetPowerBonus();
                        players[turn].health = Math.Clamp(players[turn].health + health, 0, 50);
                    }

                    int energy = lastCard.card.move.health;

                    if (energy != 0)
                    {
                        if (lastCard.card.move.moveID == 21) //special energy
                        {
                            energy *= lastCard.lastCost;
                        }

                        energy += players[1 - turn].GetPowerBonus();
                        players[turn].energy = Math.Max(players[turn].energy + energy, 0);
                    }

                    if (lastCard.card.move.effect.duration > 0)
                    {
                        players[turn].AddEffect(lastCard.card.move.effect);
                    }
                }
            }
        }

        return true;
    }

    public bool PlayLastCard(MoveMessage message)
    {
        if (!lastCard.card.hasMove || lastCard.played)
            return false;

        int cardIndex = players[message.playerIndex].cardHand[message.cardIndex];
        Card card = FightManager.singleton.players[message.playerIndex].cards[cardIndex];

        if (card.hasMove && card.move.moveType == MoveType.Response)
        {
            if (lastCard.card.move.moveID%card.move.moveID == 0)
            {
                return false;
            }
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
