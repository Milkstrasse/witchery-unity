using System.Text;
using UnityEngine;

public class FightLog
{
    private StringBuilder log;

    public FightLog()
    {
        log = new StringBuilder("Round 1 -----------------------------\n");
    }

    public void AddToLog(Card card, bool isFlipped)
    {
        if (card.hasMove)
        {
            log.Append((isFlipped ? "Flipped player played: " : "Nonflipped player played: ") + card.move.name + "\n");
        }
        else
        {
            log.Append(isFlipped ? "Flipped player played played blank card\n" : "Nonflipped player played blank card\n");
        }
    }

    public void EndRound(int round)
    {
        log.Append($"Round {round + 1} -----------------------------\n");
    }

    public string GetLog()
    {
        return log.ToString();
    }
}
