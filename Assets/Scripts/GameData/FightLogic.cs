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
        
        Card card;
        if (cardIndex < 0)
        {
            card = new Card();
        }
        else
        {
            card = FightManager.singleton.players[playerTurn].cards[cardIndex];
        }

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

            players[playerTurn].energy = players[playerTurn].energy - card.move.cost;
        }

        bool wasPlayed = PlayCard(card, playerTurn, true);
        lastCard = new PlayedCard(card, playerTurn, wasPlayed, lastCard);

        RemoveCard(message);

        return true;
    }

    public void RemoveCard(MoveMessage message)
    {
        if (!message.playCard)
        {
            int cardIndex = players[playerTurn].cardHand[message.cardIndex];

            Card card;
            if (cardIndex < 0)
            {
                card = new Card();
            }
            else
            {
                card = FightManager.singleton.players[playerTurn].cards[cardIndex];
            }

            if (card.hasMove)
            {
                players[playerTurn].energy += card.move.cost;
            }
        }

        players[playerTurn].RemoveCard(message.cardIndex);

        if (lastCard.card.hasMove && message.playCard)
        {
            switch (lastCard.card.move.moveID)
            {
                case 23: //clear blanks
                case 25: //clear blanks after handover
                    players[playerTurn].RemoveBlanks();
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
        if (move.moveID == 1 && lastCard != null) //replay last card
        {
            if (!lastCard.card.hasMove)
            {
                return true;
            }

            move = lastCard.card.move;
        }

        if (move.moveType != MoveType.Response)
        {
            if (blockable && FightManager.singleton.players[1 - turn].HasResponseTo(card.move))
            {
                return false;
            }

            switch (move.moveID)
            {
                case 1: //replay last card
                    break;
                case 4: //steal health
                    int hpToSteal = move.health;
                    if (move.moveType == MoveType.Special)
                    {
                        hpToSteal *= lastCard.card.hasMove ? lastCard.card.move.cost : 0;
                    }

                    int tempHealth = players[1 - turn].health;
                    players[1 - turn].health = Math.Max(players[1 - turn].health + hpToSteal - players[turn].GetPowerBonus() + players[1 - turn].GetDamageModifier(), 0);

                    if (winner < 0 && players[1 - turn].health == 0)
                    {
                        players[0].playedUntilEnd = true;
                        players[1].playedUntilEnd = true;

                        winner = turn;
                    }

                    int stealSpice = players[1 - turn].GetEffect("spice");
                    if (stealSpice != 0)
                    {
                        players[turn].health = Math.Max(players[turn].health - stealSpice, 0);
                        if (winner < 0 && players[turn].health == 0)
                        {
                            players[0].playedUntilEnd = true;
                            players[1].playedUntilEnd = true;

                            winner = 1 - turn;
                        }
                        else
                        {
                            tempHealth -= players[1 - turn].health;
                            if (tempHealth == 0)
                            {
                                players[turn].stoleNothing = true;
                            }

                            players[turn].health = Math.Clamp(players[turn].health + tempHealth, 0, players[turn].maxHealth);
                        }
                    }
                    else
                    {
                        tempHealth -= players[1 - turn].health;
                        if (tempHealth == 0)
                        {
                            players[turn].stoleNothing = true;
                        }

                        players[turn].health = Math.Clamp(players[turn].health + tempHealth, 0, players[turn].maxHealth);
                    }

                    break;
                case 5: //redistribute HP
                    if (players[1 - turn].health < players[turn].health)
                    {
                        players[turn].healedOpponent = true;
                    }

                    int allHealth = players[0].health + players[1].health;
                    players[0].health = Math.Clamp(allHealth/2, 0, players[0].maxHealth);
                    players[1].health = Math.Clamp(allHealth/2, 0, players[1].maxHealth);

                    break;
                case 8: //steal energy
                    int energyToSteal = move.energy;
                    if (move.moveType == MoveType.Special)
                    {
                        energyToSteal *= lastCard.card.hasMove ? lastCard.card.move.cost : 0;
                    }

                    int tempEnergy = players[1 - turn].energy;
                    players[1 - turn].energy = Math.Max(players[1 - turn].energy + energyToSteal - players[turn].GetPowerBonus(), 0);

                    tempEnergy -= players[1 - turn].energy;
                    if (tempEnergy == 0)
                    {
                        players[turn].stoleNothing = true;
                    }

                    players[turn].energy += tempEnergy;

                    break;
                case 10: //explosion
                    int damage = move.health;
                    int damageA = Math.Min(damage - players[turn].GetPowerBonus() + players[1 - turn].GetDamageModifier(), 0);
                    int damageB = Math.Min(damage - players[turn].GetPowerBonus() + players[turn].GetDamageModifier(), 0);

                    players[1 - turn].health = Math.Clamp(players[1 - turn].health + damageA, 0, players[1 - turn].maxHealth);
                    if (winner < 0 && players[1 - turn].health == 0)
                    {
                        players[0].playedUntilEnd = true;
                        players[1].playedUntilEnd = true;

                        winner = turn;
                    }

                    int exSpice = players[1 - turn].GetEffect("spice");
                    if (exSpice != 0)
                    {
                        players[turn].health = Math.Max(players[turn].health - exSpice, 0);
                        if (winner < 0 && players[turn].health == 0)
                        {
                            players[0].playedUntilEnd = true;
                            players[1].playedUntilEnd = true;

                            players[1 - turn].wonWithEffect = true;
                            winner = 1 - turn;
                        }
                    }

                    players[turn].health = Math.Clamp(players[turn].health + damageB, 0, players[turn].maxHealth);
                    if (winner < 0 && players[turn].health == 0)
                    {
                        players[0].playedUntilEnd = true;
                        players[1].playedUntilEnd = true;

                        players[turn].selfKO = true;
                        winner = 1 - turn;
                    }

                    break;
                case 11: //swap effects
                    List<StatusEffect> tempEffects = players[0].effects;
                    players[0].effects = players[1].effects;
                    players[1].effects = tempEffects;

                    break;
                case 13: //clear effects
                    if (move.target == 2)
                    {
                        players[0].effects = new List<StatusEffect>();
                        players[1].effects = new List<StatusEffect>();
                    }
                    else
                    {
                        players[(move.target + turn) % 2].effects = new List<StatusEffect>();
                    }

                    break;
                case 16: //steal effects
                    for (int i = 0; i < players[1 - turn].effects.Count; i++)
                    {
                        players[turn].AddEffect(players[1 - turn].effects[i]);
                    }

                    players[1 - turn].effects = new List<StatusEffect>();

                    break;
                case 17: //copy effects
                    players[turn].effects = players[1 - turn].effects;
                    for (int i = 0; i < players[turn].effects.Count; i++)
                    {
                        players[turn].effects[i].isNew = true;
                    }

                    break;
                case 19: //add blank
                    players[(move.target + turn) % 2].AddBlanks(1);
                    break;
                case 20: //remove random card
                    int cardAmount = players[(move.target + turn) % 2].cardHand.Count;
                    if (cardAmount > 0)
                    {
                        players[(move.target + turn) % 2].RemoveCard(UnityEngine.Random.Range(0, cardAmount));
                    }

                    break;
                case 21: //heal to health
                    players[(move.target + turn) % 2].health = Math.Max(players[(move.target + turn) % 2].health, move.health + players[turn].GetPowerBonus());
                    break;
                case 23: //clear blanks
                    break;
                case 25: //hand over blanks
                    players[1 - turn].AddBlanks(players[turn].blanks);
                    break;
                default:
                    int health = move.health;
                    if (move.moveType == MoveType.Special)
                    {
                        health *= lastCard.card.hasMove ? lastCard.card.move.cost : 0;
                    }

                    if (health < 0)
                    {
                        health = Math.Min(health - players[turn].GetPowerBonus() + players[(move.target + turn) % 2].GetDamageModifier(), 0);
                    }
                    else if (health > 0)
                    {
                        health = Math.Max(health + players[turn].GetPowerBonus(), 0);
                    }

                    players[(move.target + turn) % 2].health = Math.Clamp(players[(move.target + turn) % 2].health + health, 0, players[(move.target + turn) % 2].maxHealth);

                    if (winner < 0 && players[(move.target + turn) % 2].health == 0)
                    {
                        players[0].playedUntilEnd = true;
                        players[1].playedUntilEnd = true;

                        winner = 1 - (move.target + turn) % 2;
                    }

                    if (move.health < 0)
                    {
                        int spice = players[(move.target + turn) % 2].GetEffect("spice");
                        if (spice != 0)
                        {
                            players[turn].health = Math.Max(players[turn].health - spice, 0);
                            if (winner < 0 && players[turn].health == 0)
                            {
                                players[0].playedUntilEnd = true;
                                players[1].playedUntilEnd = true;

                                players[1 - turn].wonWithEffect = true;
                                winner = 1 - turn;
                            }
                        }
                    }

                    int energy = move.energy;
                    if (move.moveType == MoveType.Special)
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

                    players[(move.target + turn) % 2].energy += energy;

                    if (move.effect.multiplier > 0)
                    {
                        int effectAmount = move.effect.multiplier;
                        
                        if (move.moveType == MoveType.Special)
                        {
                            effectAmount *= lastCard.card.hasMove ? lastCard.card.move.cost : 0;
                        }

                        effectAmount = Math.Max(effectAmount + players[turn].GetPowerBonus(), 0);

                        StatusEffect effect = new StatusEffect(move.effect, effectAmount);
                        players[(move.target + turn) % 2].AddEffect(effect);
                    }

                    break;
            }
        }
        else if (move.moveType == MoveType.Response)
        {
            if (lastCard.card.hasMove && !lastCard.played)
            {
                if (move.moveID == 3) //take card effect
                {
                    int health = lastCard.card.move.health;

                    if (health != 0)
                    {
                        if (lastCard.card.move.moveID == 3 && lastCard.card.move.moveType == MoveType.Special) //special heal
                        {
                            health *= lastCard.lastCost;
                        }
                        else if (lastCard.card.move.moveID == 21) //heal to health
                        {
                            players[turn].health = Math.Max(players[turn].health, health + players[1 - turn].GetPowerBonus());
                            return true;
                        }

                        health = Math.Max(health + players[1 - turn].GetPowerBonus(), 0);
                        players[turn].health = Math.Clamp(players[turn].health + health, 0, players[turn].maxHealth);
                    }

                    int energy = lastCard.card.move.energy;

                    if (energy != 0)
                    {
                        if (lastCard.card.move.moveID == 9 && lastCard.card.move.moveType == MoveType.Special) //special energy
                        {
                            energy *= lastCard.lastCost;
                        }

                        energy = Math.Max(energy + players[1 - turn].GetPowerBonus(), 0);
                        players[turn].energy += energy;
                    }

                    if (lastCard.card.move.effect.multiplier > 0)
                    {
                        players[turn].AddEffect(new StatusEffect(lastCard.card.move.effect, lastCard.card.move.effect.multiplier));
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
        
        Card card;
        if (cardIndex < 0)
        {
            card = new Card();
        }
        else
        {
            card = FightManager.singleton.players[message.playerIndex].cards[cardIndex];
        }

        if (message.playCard)
        {
            if (card.hasMove && card.move.IsResponseTo(lastCard.card.move, players[message.playerIndex].energy))
            {
                return false;
            }
        }

        PlayCard(lastCard.card, lastCard.player, false);
        //---------
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
            players[i].roundsPlayed += 1;
            players[i].FillHand(5);

            int j = 0;
            while (j < players[i].effects.Count)
            {
                players[i].effects[j].TriggerEffect(players[i]);
                j++;

                if (winner < 0 && players[i].health == 0)
                {
                    players[0].playedUntilEnd = true;
                    players[1].playedUntilEnd = true;

                    players[1 - i].wonWithEffect = true;
                    winner = 1 - i;
                }
            }
        }
    }
}
