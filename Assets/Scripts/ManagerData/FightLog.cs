using System.Text;
using UnityEngine;

public class FightLog
{
    private StringBuilder log;

    public FightLog()
    {
        log = new StringBuilder("Round 1 -----------------------------\n");
    }

    public void AddToLog(CardUI cardUI, bool isRotated)
    {
        log.Append($"{GlobalData.fighters[cardUI.player.fighterIDs[0].fighterID].name} has {cardUI.player.energy} energy & {cardUI.player.currHealth}/{cardUI.player.fullHealth}HP\n");
        if (cardUI.card.hasMove)
        {
            log.Append((isRotated ? "Flipped player played: " : "Nonflipped player played: ") + cardUI.card.move.name + "\n");
        }
        else
        {
            log.Append(isRotated ? "Flipped player played played blank card\n" : "Nonflipped player played blank card\n");
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