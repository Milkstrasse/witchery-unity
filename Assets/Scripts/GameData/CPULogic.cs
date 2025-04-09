using System;
using System.Collections.Generic;

public struct CPULogic
{
    public static MoveMessage GetMove(PlayerObject player, FightLogic logic)
    {
        List<(int, int)> prioritizedCards = new List<(int, int)>();
        float missingHP = 1 - player.currHealth / player.fullHealth;

        for (int i = 0; i < player.cardHand.Count; i++)
        {
            if (!player.cardHand[i].hasMove)
            {
                prioritizedCards.Add((i, 0));
            }
            else if (player.cardHand[i].hasMove && player.cardHand[i].move.cost <= player.energy)
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
                    prioritizedCards.Add((i, -10));
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
                        prioritizedCards.Add((i, -15 + player.cardHand[i].move.cost)); //get biggest amount of resources back
                        continue;
                    }

                    health = Math.Min(health - logic.players[1].GetPowerBonus() + logic.players[0].GetDamageModifier(), 0);

                    if (logic.players[0].health + health <= 0) //opponent defeated
                    {
                        return new MoveMessage(1, i, true);
                    }
                    else
                    {
                        if ((move.moveID == 10 && player.currHealth + health <= 0) || logic.players[0].GetEffect("spice") >= player.currHealth) //prevent self k.o.
                        {
                            prioritizedCards.Add((i, -15 + player.cardHand[i].move.cost)); //get biggest amount of resources back
                        }
                        else if (health == 0 && player.cardHand[i].move.cost > 0)
                        {
                            prioritizedCards.Add((i, -15 + player.cardHand[i].move.cost)); //get biggest amount of resources back
                        }
                        else
                        {
                            prioritizedCards.Add((i, health * -1));
                        }
                    }
                }
                else
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
                                health = Math.Max(health + logic.players[1].GetPowerBonus(), 0);

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
                                    prioritizedCards.Add((i, -15 + player.cardHand[i].move.cost)); //get biggest amount of resources back
                                }
                                else
                                {
                                    prioritizedCards.Add((i, health));
                                }

                                break;
                            }
                            else if (missingHP < 0.1f) //excessive healing
                            {
                                prioritizedCards.Add((i, -15 + player.cardHand[i].move.cost)); //get biggest amount of resources back
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
                                    prioritizedCards.Add((i, player.cardHand[i].move.cost * -1 + stealEnergy * -40)); //prioritize energy & cheap cards
                                }
                                else
                                {
                                    stealEnergy -= checkEnergy;
                                    prioritizedCards.Add((i, player.cardHand[i].move.cost * -1 + stealEnergy * -40)); //prioritize energy & cheap cards
                                }
                            }

                            break;
                        case 9: //energy
                            if (move.moveType == MoveType.Special && logic.lastCard.card.hasMove)
                            {
                                int energy = move.energy * logic.lastCard.card.move.cost + logic.players[1].GetPowerBonus();

                                if (energy >= player.cardHand[i].move.cost)
                                {
                                    prioritizedCards.Add((i, player.cardHand[i].move.cost * -1 + move.energy * 40)); //prioritize energy & cheap cards
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
                            if (logic.players[1].blanks == 0)
                            {
                                goto case 0;
                            }
                            else
                            {
                                goto default;
                            }
                        case 0:
                            prioritizedCards.Add((i, -15 + player.cardHand[i].move.cost)); //get biggest amount of resources back
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
                                prioritizedCards.Add((i, player.cardHand[i].move.cost * -1 + move.energy * 40)); //prioritize energy & cheap cards
                                break;
                            }


                    }
                }
            }
            else //unplayable cards
            {
                prioritizedCards.Add((i, -15 + player.cardHand[i].move.cost)); //get biggest amount of resources back
            }
        }

        (int, int)[] cards = prioritizedCards.ToArray();
        Array.Sort(cards, (a, b) => { return b.Item2.CompareTo(a.Item2); });

        return new MoveMessage(1, cards[0].Item1, cards[0].Item2 > -10);
    }
}