using System;
using System.Collections.Generic;
using UnityEngine;

public struct CPULogic
{
    private List<(int, int)> prioritizedCards;
    private int playerIndex;
    private int opponentIndex;

    public CPULogic(int playerIndex, int opponentIndex)
    {
        this.playerIndex = playerIndex;
        this.opponentIndex = opponentIndex;

        prioritizedCards = new List<(int, int)>();
    }

    //evaluate all cards in cardhand to determine best suitable move
    public MoveMessage GetMove(PlayerObject player, FightLogic logic)
    {
        prioritizedCards = new List<(int, int)>();
        float missingHP = 1 - player.currHealth / (float)player.fullHealth;

        Debug.Log($"CPU has {player.energy} energy & is missing {missingHP} HP");
        Debug.Log(logic.lastCard.card.hasMove ? $"Last card costs {logic.lastCard.card.move.cost} energy, moveID is {logic.lastCard.card.move.moveID}" : $"Last card costs 0 energy, moveID is 0");

        for (int i = 0; i < player.cardHand.Count; i++)
        {
            if (!player.cardHand[i].hasMove)
            {
                Debug.Log("No move");
            }
            else
            {
                Debug.Log(player.cardHand[i].move.name);
            }
        }

        for (int i = 0; i < player.cardHand.Count; i++)
        {
            if (!player.cardHand[i].hasMove)
            {
                prioritizedCards.Add((i, 0));
            }
            else
            {
                Move move = player.cardHand[i].move;
                if (move.moveID == 1 && logic.lastCard.card.hasMove) //replay card
                {
                    move = logic.lastCard.card.move;
                }

                if (logic.lastCard.card.hasMove)
                {
                    if (!logic.lastCard.played && move.IsResponseTo(logic.lastCard.card.move))
                    {
                        prioritizedCards.Add((i, 999));
                        continue;
                    }
                    else if (logic.lastCard.card.move.moveID == 7 && move.moveID % 2 == 0)
                    {
                        GetMostResourcesBack(player, i, logic.lastCard.card.move.cost);
                        continue;
                    }
                }

                if (move.moveType == MoveType.Response)
                {
                    prioritizedCards.Add((i, -20));
                    continue;
                }

                int health = move.health;
                if (move.moveID >= 2 && move.moveID <= 4 && move.moveType == MoveType.Special) //special moves
                {
                    health *= logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0;
                }

                if (health < 0) //all attacks
                {
                    int finalhealth = Math.Min(health - player.GetPowerBonus() + logic.players[opponentIndex].GetDamageModifier(false), 0);

                    if (logic.players[opponentIndex].currHealth + finalhealth <= 0) //opponent could be defeated
                    {
                        if (player.cardHand[i].move.cost > player.energy) //check if card could be played if there's enough energy
                        {
                            List<(int, int)> potentialCards = new List<(int, int)>();

                            int potential = 0;
                            for (int j = 0; j < player.cardHand.Count; j++)
                            {
                                if (i != j)
                                {
                                    potential += player.cardHand[j].move.cost; //prevent opponent's reaction by creating energy through discard
                                    potentialCards.Add((j, player.cardHand[j].move.cost));
                                }
                            }

                            if (potential >= player.cardHand[i].move.cost)
                            {
                                (int, int)[] potentials = potentialCards.ToArray();
                                Array.Sort(potentials, (a, b) => { return b.Item2.CompareTo(a.Item2); });

                                Debug.Log("-----------------");
                                Debug.Log("Could defeat opponent");
                                Debug.Log("-----------------");

                                return new MoveMessage(playerIndex, potentials[0].Item1, false);
                            }
                            else
                            {
                                GetMostResourcesBack(player, i, logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0);
                                continue;
                            }
                        }

                        Debug.Log("-----------------");
                        Debug.Log("Defeat opponent");
                        Debug.Log("-----------------");
                        return new MoveMessage(playerIndex, i, true);
                    }
                    else if ((move.moveID == 10 && player.currHealth + health - Math.Min(player.GetPowerBonus() + logic.players[playerIndex].GetDamageModifier(false), 0) - logic.players[opponentIndex].GetEffect("spice", false) <= 0) || logic.players[opponentIndex].GetEffect("spice", false) >= player.currHealth) //prevent self k.o.
                    {
                        GetMostResourcesBack(player, i, logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0);
                    }
                    else if (player.cardHand[i].move.cost > player.energy)
                    {
                        GetMostResourcesBack(player, i, logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0);
                    }
                    else
                    {
                        prioritizedCards.Add((i, health * -10));
                    }
                }
                else if (move.moveID == 26) //trigger curse
                {
                    int curse = logic.players[opponentIndex].GetEffect("curse", false);

                    if (curse == 0)
                    {
                        GetMostResourcesBack(player, i, logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0);
                    }
                    else if (curse >= logic.players[opponentIndex].currHealth)
                    {
                        if (player.cardHand[i].move.cost > player.energy)
                        {
                            List<(int, int)> potentialCards = new List<(int, int)>();

                            int potential = 0;
                            for (int j = 0; j < player.cardHand.Count; j++)
                            {
                                if (i != j)
                                {
                                    potential += player.cardHand[j].move.cost; //prevent opponent's reaction by creating energy through discard
                                    potentialCards.Add((j, player.cardHand[j].move.cost));
                                }
                            }

                            if (potential >= player.cardHand[i].move.cost)
                            {
                                (int, int)[] potentials = potentialCards.ToArray();
                                Array.Sort(potentials, (a, b) => { return b.Item2.CompareTo(a.Item2); });

                                Debug.Log("-----------------");
                                Debug.Log("Could defeat opponent");
                                Debug.Log("-----------------");
                                return new MoveMessage(playerIndex, potentials[0].Item1, false);
                            }
                            else
                            {
                                GetMostResourcesBack(player, i, logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0);
                                continue;
                            }
                        }

                        Debug.Log("-----------------");
                        Debug.Log("Defeat opponent");
                        Debug.Log("-----------------");
                        return new MoveMessage(playerIndex, i, true);
                    }
                    else if (player.cardHand[i].move.cost <= player.energy)
                    {
                        prioritizedCards.Add((i, curse * -10));
                    }
                    else
                    {
                        GetMostResourcesBack(player, i, logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0);
                    }
                }
                else if (player.cardHand[i].move.cost <= player.energy)
                {
                    switch (move.moveID)
                    {
                        case 3: // heal
                            if (health > 0 && missingHP > 0.6f)
                            {
                                health = Math.Max(health + player.GetPowerBonus(), 0);

                                if (move.moveID == 21) //heal to health
                                {
                                    health = Math.Max(health - player.currHealth, 0);
                                }

                                if (player.currHealth + health > player.fullHealth) //excessive healing
                                {
                                    health = player.fullHealth - player.currHealth;
                                }

                                if (health == 0 && player.cardHand[i].move.cost > 0)
                                {
                                    goto case 0;
                                }
                                else
                                {
                                    prioritizedCards.Add((i, health * 10));
                                    break;
                                }
                            }
                            else if (missingHP < 0.1f) //excessive healing
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 5: //redistribute health
                            if (player.currHealth >= logic.players[opponentIndex].currHealth)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 7: //prevent attack
                            if (logic.players[opponentIndex].cardHand.Count == 0 && player.cardHand.Count > 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 8: //steal energy
                            if (logic.players[opponentIndex].energy == 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                int stealEnergy = move.energy;

                                if (move.moveType == MoveType.Special)
                                {
                                    stealEnergy *= logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0;
                                }

                                stealEnergy -= player.GetPowerBonus();

                                if (stealEnergy != 0)
                                {
                                    int checkEnergy = logic.players[opponentIndex].energy + stealEnergy;

                                    if (checkEnergy >= 0)
                                    {
                                        PrioritizeEnergyOrCheap(player, i, -stealEnergy);
                                    }
                                    else
                                    {
                                        stealEnergy -= checkEnergy;
                                        PrioritizeEnergyOrCheap(player, i, -stealEnergy);
                                    }
                                }
                                else
                                {
                                    goto case 0;
                                }
                            }

                            break;
                        case 9: //energy
                            if (move.moveType == MoveType.Special && logic.lastCard.card.hasMove)
                            {
                                int energy = move.energy * logic.lastCard.card.move.cost + player.GetPowerBonus();

                                if (energy >= player.cardHand[i].move.cost)
                                {
                                    PrioritizeEnergyOrCheap(player, i, energy);
                                    break;
                                }
                                else
                                {
                                    goto case 0;
                                }
                            }
                            else if (move.moveType == MoveType.Special)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 11: //swap effects
                            if (logic.players[playerIndex].CheckEffectBalance() >= logic.players[opponentIndex].CheckEffectBalance())
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 13: //clear effects
                            if (move.target != 1 && logic.players[playerIndex].CheckEffectBalance() >= 0) //clear own effects
                            {
                                goto case 0;
                            }
                            else if (move.target == 1 && logic.players[opponentIndex].CheckEffectBalance() <= 0) //clear opponent's effects
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 14: //apply effect
                            if (move.moveType == MoveType.Special && (!logic.lastCard.card.hasMove || logic.lastCard.card.move.cost == 0))
                            {
                                goto case 0;
                            }
                            else if (logic.players[opponentIndex].GetEffect(move.effect.name, false) / move.effect.value >= GlobalData.customStackLimit)
                            {
                                goto case 0;
                            }
                            else if (logic.players[opponentIndex].effects.Count == GlobalData.effectLimit && logic.players[opponentIndex].GetEffect(move.effect.name, false) == 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                PrioritizeEnergyOrCheap(player, i, move.energy);
                                break;
                            }
                        case 15: //gain effect
                            if (move.moveType == MoveType.Special && (!logic.lastCard.card.hasMove || logic.lastCard.card.move.cost == 0))
                            {
                                goto case 0;
                            }
                            else if (logic.players[playerIndex].effects.Count == GlobalData.effectLimit && logic.players[playerIndex].GetEffect(move.effect.name, false) == 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                PrioritizeEnergyOrCheap(player, i, move.energy);
                                break;
                            }
                        case 16: //steal effects
                            if (logic.players[opponentIndex].CheckEffectBalance() <= 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 20: //remove random card
                            if (logic.players[opponentIndex].cardHand.Count == 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 22: //add blank
                            if (logic.players[opponentIndex].blanks >= GlobalData.customBlankLimit)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 23: //clear blanks
                        case 25: //hand over blanks
                            if (player.blanks == 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 0:
                            GetMostResourcesBack(player, i, logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0);
                            break;
                        default:
                            if (move.moveType == MoveType.Special && (!logic.lastCard.card.hasMove || logic.lastCard.card.move.cost == 0))
                            {
                                goto case 0;
                            }
                            else if (move.moveType == MoveType.Special && move.energy * logic.lastCard.card.move.cost <= player.cardHand[i].move.cost)
                            {
                                goto case 0;
                            }
                            else
                            {
                                PrioritizeEnergyOrCheap(player, i, move.energy);
                                break;
                            }
                    }
                }
                else //unplayable cards
                {
                    GetMostResourcesBack(player, i, logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0);
                }
            }
        }

        (int, int)[] cards = prioritizedCards.ToArray();
        Array.Sort(cards, (a, b) => { return b.Item2.CompareTo(a.Item2); });

        Debug.Log("-----------------");
        for (int i = 0; i < cards.Length; i++)
        {
            Debug.Log(player.cardHand[cards[i].Item1].hasMove ? player.cardHand[cards[i].Item1].move.name : "No move");
            Debug.Log(cards[i].Item2);
        }
        Debug.Log("-----------------");

        return new MoveMessage(playerIndex, cards[0].Item1, cards[0].Item2 > -10);
    }

    private void GetMostResourcesBack(PlayerObject player, int cardIndex, int lastCost)
    {
        if (lastCost < 2)
        {
            prioritizedCards.Add((cardIndex, -15 + player.cardHand[cardIndex].move.cost - (player.cardHand[cardIndex].move.moveType == MoveType.Special ? 0 : 1))); //prioritize discarding special cards
        }
        else
        {
            prioritizedCards.Add((cardIndex, -15 + player.cardHand[cardIndex].move.cost - (player.cardHand[cardIndex].move.moveType == MoveType.Special ? 1 : 0))); //prioritize discarding normal cards
        }
    }

    private void PrioritizeEnergyOrCheap(PlayerObject player, int cardIndex, int energy)
    {
        prioritizedCards.Add((cardIndex, player.cardHand[cardIndex].move.cost * -1 + energy * 10)); //prioritize energy & cheap cards
    }
}