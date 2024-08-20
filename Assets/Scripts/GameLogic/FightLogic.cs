using System;

public class FightLogic
{
    public int playerTurn;
    public PlayedCard lastCard;

    public void PlayCard(Card card, Player[] players)
    {
        int[] indices = new int[]{playerTurn, 1 - playerTurn};
        for (int i = 0; i < indices.Length; i++)
        {
            Move move = GlobalManager.singleton.fighters[card.fighterID].moves[card.moveID];

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

    public bool IsGameOver(Player[] players)
    {
        return players[0].currHealth == 0 || players[01].currHealth == 0;
    }
}

public struct PlayedCard
{
    public Card card;
    public int playerIndex;

    public PlayedCard(int playerIndex, Card card)
    {
        this.card = card;
        this.playerIndex = playerIndex;
    }
}
