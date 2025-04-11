using System;
using System.Collections.Generic;

public struct CPULogic
{
    private static List<(int, int)> prioritizedCards;

    public static MoveMessage GetMove(PlayerObject player, FightLogic logic)
    {
        prioritizedCards = new List<(int, int)>();
        float missingHP = 1 - player.currHealth / player.fullHealth;

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

                if (logic.lastCard.card.hasMove && !logic.lastCard.played)
                {
                    if (move.IsResponseTo(logic.lastCard.card.move, player.energy))
                    {
                        prioritizedCards.Add((i, 999));
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
                    if (logic.lastCard.card.hasMove && logic.lastCard.card.move.moveID == 7)
                    {
                        GetMostResourcesBack(player, i);
                        continue;
                    }

                    health = Math.Min(health - player.GetPowerBonus() + logic.players[0].GetDamageModifier(false), 0);

                    if (logic.players[0].health + health <= 0) //opponent could be defeated
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
                                return new MoveMessage(1, potentials[0].Item1, false);
                            }
                            else
                            {
                                GetMostResourcesBack(player, i);
                                continue;
                            }
                        }

                        return new MoveMessage(1, i, true);
                    }
                    else if ((move.moveID == 10 && player.currHealth + health + logic.players[1].GetDamageModifier(false) <= 0) || logic.players[0].GetEffect("spice", false) >= player.currHealth) //prevent self k.o.
                    {
                        GetMostResourcesBack(player, i);
                    }
                    else if (health == 0 && player.cardHand[i].move.cost > 0)
                    {
                        GetMostResourcesBack(player, i);
                    }
                    else if (player.cardHand[i].move.cost > player.energy)
                    {
                        GetMostResourcesBack(player, i);
                    }
                    else
                    {
                        prioritizedCards.Add((i, health * -1));
                    }
                }
                else if (player.cardHand[i].move.cost <= player.energy)
                {
                    switch (move.moveID)
                    {
                        case 1: //replay card
                            if (!logic.lastCard.card.hasMove || logic.lastCard.card.move.moveID == 1)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
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
                                    GetMostResourcesBack(player, i);
                                }
                                else
                                {
                                    prioritizedCards.Add((i, health));
                                }

                                break;
                            }
                            else if (missingHP < 0.1f) //excessive healing
                            {
                                GetMostResourcesBack(player, i);
                                break;
                            }
                            else
                            {
                                goto default;
                            }
                        case 5: //redistribute health
                            if (player.currHealth >= logic.players[0].health)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 8: //steal energy
                            int stealEnergy = move.energy;

                            if (logic.players[0].energy == 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                if (move.moveType == MoveType.Special)
                                {
                                    stealEnergy *= logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0;
                                }

                                stealEnergy -= player.GetPowerBonus();

                                int checkEnergy = logic.players[0].energy + stealEnergy;

                                if (checkEnergy >= 0)
                                {
                                    PrioritizeEnergyOrCheap(player, i, stealEnergy);
                                }
                                else
                                {
                                    stealEnergy -= checkEnergy;
                                    PrioritizeEnergyOrCheap(player, i, stealEnergy);
                                }
                            }

                            break;
                        case 9: //energy
                            if (move.moveType == MoveType.Special && logic.lastCard.card.hasMove)
                            {
                                int energy = move.energy * logic.lastCard.card.move.cost + player.GetPowerBonus();

                                if (energy >= player.cardHand[i].move.cost)
                                {
                                    PrioritizeEnergyOrCheap(player, i, move.energy);
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
                            if (logic.players[1].CheckEffectBalance() >= logic.players[0].CheckEffectBalance())
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 13: //clear effects
                            if (move.target != 1 && logic.players[1].CheckEffectBalance() >= 0) //clear own effects
                            {
                                goto case 0;
                            }
                            else if (move.target == 1 && logic.players[0].CheckEffectBalance() <= 0) //clear opponent's effects
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
                            else if (logic.players[0].GetEffect(move.effect.name, false) / move.effect.value >= GlobalData.stackLimit)
                            {
                                goto case 0;
                            }
                            else if (logic.players[0].effects.Count == GlobalData.effectLimit && logic.players[0].GetEffect(move.effect.name, false) == 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 15: //gain effect
                            if (move.moveType == MoveType.Special && (!logic.lastCard.card.hasMove || logic.lastCard.card.move.cost == 0))
                            {
                                goto case 0;
                            }
                            else if (logic.players[1].effects.Count == GlobalData.effectLimit && logic.players[1].GetEffect(move.effect.name, false) == 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 16: //steal effects
                            if (logic.players[0].CheckEffectBalance() <= 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 20: //remove random card
                            if (logic.players[0].cardHand.Count == 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 22: //add blank
                            if (logic.players[0].blanks >= GlobalData.blankLimit)
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
                            GetMostResourcesBack(player, i);
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
                    GetMostResourcesBack(player, i);
                }
            }
        }

        (int, int)[] cards = prioritizedCards.ToArray();
        Array.Sort(cards, (a, b) => { return b.Item2.CompareTo(a.Item2); });

        return new MoveMessage(1, cards[0].Item1, cards[0].Item2 > -10);
    }

    private static void GetMostResourcesBack(PlayerObject player, int cardIndex)
    {
        prioritizedCards.Add((cardIndex, -15 + player.cardHand[cardIndex].move.cost)); //get biggest amount of resources back
    }

    private static void PrioritizeEnergyOrCheap(PlayerObject player, int cardIndex, int energy)
    {
        prioritizedCards.Add((cardIndex, player.cardHand[cardIndex].move.cost * -1 + energy * -40)); //prioritize energy & cheap cards
    }
}