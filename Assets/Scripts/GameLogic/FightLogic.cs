using System;

public class FightLogic
{
    public int playerTurn;
    public PlayedCard lastCard;

    public FightLogic()
    {
        lastCard.played = true;
    }

    public bool PlayCard(Card card, Player[] players)
    {
        Move move = GlobalManager.singleton.fighters[card.fighterID].moves[card.moveID];

        int[] indices = new int[]{playerTurn, 1 - playerTurn};

        if (players[indices[1]].HasResponse(move))
        {
            return false;
        }

        for (int i = 0; i < 2; i++)
        {
            int health = move.health[i];
            if (health != 0)
            {
                players[indices[i]].currHealth = Math.Clamp(players[indices[i]].currHealth + health, 0, players[indices[i]].fullHealth);
            }

            StatusEffect statusEffect = StatusEffect.GetStatus(move.statusEffects[i]);
            if (statusEffect.duration > 0 && players[indices[i]].effects.Count < 5)
            {
                players[indices[i]].effects.Add(statusEffect);
            }
        }

        return true;
    }

    public void PlayLastCard(Player[] players)
    {
        Move move = GlobalManager.singleton.fighters[lastCard.card.fighterID].moves[lastCard.card.moveID];
        int[] indices = new int[]{lastCard.playerIndex, 1 - lastCard.playerIndex};
        for (int i = 0; i < 2; i++)
        {
            int health = move.health[i];
            if (health != 0)
            {
                players[indices[i]].currHealth = Math.Clamp(players[indices[i]].currHealth + health, 0, players[indices[i]].fullHealth);
            }

            StatusEffect statusEffect = StatusEffect.GetStatus(move.statusEffects[i]);
            if (statusEffect.duration > 0 && players[indices[i]].effects.Count < 5)
            {
                players[indices[i]].effects.Add(statusEffect);
            }
        }
    }

    public bool IsResponse(Card card)
    {
        Move move = GlobalManager.singleton.fighters[card.fighterID].moves[card.moveID];
        return move.moveType == MoveType.Response;
    }

    public bool IsGameOver(Player[] players)
    {
        if (players[0].currHealth == 0)
        {
            players[0].gaveUp = true;
            return true;
        }
        if (players[1].currHealth == 0)
        {
            players[1].gaveUp = true;
            return true;
        }

        return false;
    }
}

public struct PlayedCard
{
    public Card card;
    public int playerIndex;
    public bool played;

    public PlayedCard(int playerIndex, Card card, bool played)
    {
        this.card = card;
        this.playerIndex = playerIndex;
        this.played = played;
    }
}
