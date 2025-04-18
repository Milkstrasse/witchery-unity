using System;

[Serializable]
public struct SavedData
{
    public FighterData[] fighters;
    public bool[] missions;

    public int timesFought;
    public int timesWon;
    public int timesFoughtFirst;
    public int timesWonFirst;
    public int damageDone;
    public int healingDone;
    public int energyCreated;
    public int healingStolen;
    public int energyStolen;
    public int effectsApplied;
    public int timesReplayed;
    public int timesBlocked;
    public int timesTaken;
    public bool nothingStolen;
    public int maxEffectCount;
    public bool healedOpponent;
    public int maxDamageDone;
    public int maxHealingDone;
    public int wonWithMinHealth;
    public bool wonWithEffect;
}