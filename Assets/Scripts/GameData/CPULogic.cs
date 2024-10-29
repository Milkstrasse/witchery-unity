using System;
using System.Collections.Generic;
using UnityEngine;

public struct CPULogic
{
    public static MoveMessage GetMove(Player player, FightLogic logic)
    {
        List<(int, int)> prioritizedCards = new List<(int, int)>();

        for (int i = 0; i < player.cardHand.Count; i++)
        {
            if (!player.cardHand[i].hasMove)
            {
                prioritizedCards.Add((i, 0));
            }
            else if (player.cardHand[i].hasMove && player.cardHand[i].move.cost <= player.energy)
            {
                Move move = player.cardHand[i].move;
                if (move.moveID == 7 && logic.lastCard.card.hasMove) //replay card
                {
                    move = logic.lastCard.card.move;
                }

                if (logic.lastCard.card.hasMove && !logic.lastCard.played)
                {
                    if (player.IsResponse(logic.lastCard.card.move, move))
                    {
                        prioritizedCards.Add((i, 50));
                        continue;
                    }
                }

                int health = move.health;
                if (move.moveID >= 8 && move.moveID <= 10) //special moves
                {
                    health *= logic.lastCard.card.hasMove ? logic.lastCard.card.move.cost : 0;
                }

                if (health < 0)
                {
                    health = Math.Min(health - logic.players[1].GetPowerBonus() + logic.players[0].GetDamageModifier(), 0);

                    if (logic.players[0].health + health <= 0) //opponent defeated
                    {
                        return new MoveMessage(1, i, true);
                    }
                    else
                    {
                        prioritizedCards.Add((i, health * -1 + 30));
                    }
                }
                else if (health > 0)
                {
                    if (move.moveID == 6) //heal to health
                    {
                        health = Math.Max(health + logic.players[1].GetPowerBonus() - logic.players[1].health, 0);
                    }
                    else
                    {
                        health = Math.Max(health + logic.players[1].GetPowerBonus(), 0);
                    }

                    if (logic.players[1].health + health > GlobalSettings.health) //excessive healing
                    {
                        prioritizedCards.Add((i, -1 - move.cost));
                    }
                    else
                    {
                        prioritizedCards.Add((i, health));
                    }
                }
                else
                {
                    if (move.moveID == 2 && logic.players[1].health > logic.players[0].health) //redistribute health
                    {
                        prioritizedCards.Add((i, -10));
                    }
                    else if (move.effect.duration > 0 &&  logic.players[1 - move.target].effects.Count == 5)
                    {
                        if (move.effect.isDelayed || logic.players[1 - move.target].GetEffect(move.effect.name) == null) //effect can't be added
                        {
                            prioritizedCards.Add((i, -10));
                        }
                        else
                        {
                            prioritizedCards.Add((i, move.cost * -1));
                        }
                    }
                    else
                    {
                        prioritizedCards.Add((i, move.cost * -1 + move.energy));
                    }
                }
            }
            else //unplayable cards
            {
                prioritizedCards.Add((i, -10));
            }
        }

        (int, int)[] cards = prioritizedCards.ToArray();
        Array.Sort(cards, (a,b) => { return b.Item2.CompareTo(a.Item2); });

        //MoveMessage(int playerIndex, int cardIndex, bool playCard, bool cardPlayed = false)
        return new MoveMessage(1, cards[0].Item1, cards[0].Item2 > -10);
    }

    /*
    static func getCardIndex(player: Player, opponent: Player, playedCard: PlayedCard?, cardToPlay: Bool) -> (Int, Bool) {
        var cards: [SelectedFighter] = []
        
        for index in player.cardHand.indices {
            var card: Card = player.cardHand[index]
            
            if playedCard != nil {
                if card.spell.type == 12 || (cardToPlay && card.spell.type == 10) { //copy card or reflect card
                    card = playedCard!.card
                }
            }
            
            if player.mana >= card.spell.cost {
                var spellDamage: Int = card.spell.damage[1]
                switch card.spell.type {
                case 3: //hex
                    spellDamage *= player.hexes.count * 4
                case 4: //card
                    spellDamage *= (playedCard?.card.spell.cost ?? 0) * 4
                default:
                    break
                }
                
                var healAmount: Int = card.spell.damage[0]
                switch card.spell.type {
                case 3: //hex
                    healAmount *= player.hexes.count * 4
                case 4: //card
                    healAmount *= (playedCard?.card.spell.cost ?? 0) * 4
                default:
                    break
                }
                
                if player.getDamage(damage: spellDamage) >= opponent.currHealth + opponent.getShields() { //defeat opponent
                    return (index, true)
                } else if card.spell.type == 9 && playedCard != nil && !playedCard!.played && playedCard!.card.spell.damage[1] > 0 { //block damage
                    cards.append((index, 100))
                } else if card.spell.type == 10 && playedCard != nil && !playedCard!.played && playedCard!.card.spell.hex[1] != nil { //reflect hex
                    cards.append((index, 100))
                } else if Float(player.currHealth)/Float(player.maxHealth) < 0.35 && (card.spell.damage[0] < 0 || (card.spell.hex[0] == Hexes.healing && !player.hasHex(hex: Hexes.healing))) { //heal
                    cards.append((index, 80 - healAmount))
                } else if card.spell.type == 17 && player.curses > 0 { //cleanse
                    cards.append((index, 50))
                } else {
                    var priority: Int = player.getDamage(damage: spellDamage)
                    if card.spell.hex[0] != nil || card.spell.hex[1] != nil {
                        if card.spell.hex[1] != Hexes.bomb || !opponent.hasHex(hex: Hexes.bomb) {
                            priority += 1
                        }
                    }
                    
                    cards.append((index, priority))
                }
            }
        }
        
        cards.sort { $0.1 > $1.1 }
        
        let cardIndex: Int = cards.first?.0 ?? Int.random(in: 0 ..< player.cardHand.count)
        
        var card: Card = player.cardHand[cardIndex]
        
        if player.mana >= card.spell.cost || player.cardHand[cardIndex].spell.type == 13 {
            if cardIndex == player.cardHand.count - 1 && player.critCounter == 0 && card.spell.type < 5 && card.spell.mana[0] > 0 { //no need to waste crit to gain mana if no other cards are left
                return (cardIndex, false)
            } else if card.spell.type == 11 && player.currHealth >= opponent.currHealth { //don't give opponent health back
                return (cardIndex, false)
            } else if card.spell.type == 6 && player.hasPositiveHexes() { //don't remove good hexes
                return (cardIndex, false)
            } else if card.spell.type == 7 && !opponent.hasPositiveHexes() { //don't get negative hexes
                return (cardIndex, false)
            } else {
                return (cardIndex, true)
            }
        } else {
            return (cardIndex, false)
        }
    }
    */
}