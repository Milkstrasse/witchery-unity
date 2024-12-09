using System;
using System.Collections.Generic;

public struct CPULogic
{
    public static MoveMessage GetMove(PlayerObject player, FightLogic logic)
    {
        List<(int, int)> prioritizedCards = new List<(int, int)>();
        float missingHP = 1 - player.currHealth/player.fullHealth;

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
                        prioritizedCards.Add((i, 50));
                        continue;
                    }
                }

                if (move.moveType == MoveType.Response)
                {
                    prioritizedCards.Add((i, -10));
                    continue;
                }

                int health = move.health;
                if ((move.moveID == 2 || move.moveID == 3) && move.moveType == MoveType.Special) //special moves
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
                        prioritizedCards.Add((i, health * -1));
                    }
                }
                else if (health > 0 && missingHP > 0.6)
                {
                    if (move.moveID == 21) //heal to health
                    {
                        health = Math.Max(health + logic.players[1].GetPowerBonus() - player.currHealth, 0);
                    }
                    else
                    {
                        health = Math.Max(health + logic.players[1].GetPowerBonus(), 0);
                    }

                    if (player.currHealth + health > player.fullHealth) //excessive healing
                    {
                        health = player.fullHealth - player.currHealth;
                    }
                    
                    prioritizedCards.Add((i, health));
                }
                else
                {
                    if (move.moveID == 5 && player.currHealth > logic.players[0].health) //redistribute health
                    {
                        prioritizedCards.Add((i, -10));
                    }
                    else if (move.moveID == 7 && logic.players[0].cardHand.Count == 0) //remove random card
                    {
                        prioritizedCards.Add((i, -10));
                    }
                    else if (move.moveID == 11 && logic.players[1].CheckEffectBalance() >= logic.players[0].CheckEffectBalance()) //swap effects
                    {
                        prioritizedCards.Add((i, -10));
                    }
                    else if (move.moveID == 13 && move.target != 1 && logic.players[1].CheckEffectBalance() >= 0) //clear own effects
                    {
                        prioritizedCards.Add((i, -10));
                    }
                    else if (move.moveID == 13 && move.target == 1 && logic.players[0].CheckEffectBalance() < 0) //clear opponent's effects
                    {
                        prioritizedCards.Add((i, -10));
                    }
                    else if (move.moveID == 14 && logic.players[0].effects.Count == 5 && logic.players[0].GetEffect(move.effect.name, false) == 0) //apply effect
                    {
                        prioritizedCards.Add((i, -10));
                    }
                    else if (move.moveID == 15 && logic.players[1].effects.Count == 5 && logic.players[1].GetEffect(move.effect.name, false) == 0) //gain effect
                    {
                        prioritizedCards.Add((i, -10));
                    }
                    else if ((move.moveID == 23 || move.moveID == 25) && logic.players[1].blanks == 0) //hand over or clear blanks
                    {
                        prioritizedCards.Add((i, -10));
                    }
                    else
                    {
                        prioritizedCards.Add((i, move.cost * -1 + move.energy * 40)); //prioritize energy & cheap cards
                    }
                }
            }
            else //unplayable cards
            {
                prioritizedCards.Add((i, -15 + player.cardHand[i].move.cost)); //get biggest amount of resources back
            }
        }

        (int, int)[] cards = prioritizedCards.ToArray();
        Array.Sort(cards, (a,b) => { return b.Item2.CompareTo(a.Item2); });

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