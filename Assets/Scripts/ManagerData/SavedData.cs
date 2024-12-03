using System;
using UnityEngine;

[Serializable]
public struct SavedData
{
    public string name;
    public int icon;
    public int money;
    public bool[,] unlocked;
    public bool[] missions;
    public SelectedFighter[] shopFighters;

    public int timesFought;
    public int timesWon;
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
    public int moneySpent;
    public bool nothingStolen;
    public int maxEffectCount;
    public bool healedOpponent;
    public int maxDamageDone;
    public int maxHealingDone;
    public int wonWithMinHealth;
    public bool wonWithEffect;
    public bool selfKO;
}
