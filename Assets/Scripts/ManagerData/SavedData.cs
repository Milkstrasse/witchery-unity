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
    public int winWithMinHealth;
    public int winWithEffect;

    public SavedData(string name, int icon)
    {
        this.name = name;
        this.icon = icon;

        money = 0;
        unlocked = new bool[0, 0];
        missions = new bool[0];
        shopFighters = new SelectedFighter[0];

        timesFought = 0;
        timesWon = 0;
        timesWonFirst = 0;
        damageDone = 0;
        healingDone = 0;
        energyCreated = 0;
        healingStolen = 0;
        energyStolen = 0;
        effectsApplied = 0;
        timesReplayed = 0;
        timesBlocked = 0;
        timesTaken = 0;
        moneySpent = 0;
        nothingStolen = false;
        maxEffectCount = 0;
        healedOpponent = false;
        maxDamageDone = 0;
        maxHealingDone = 0;
        winWithMinHealth = 0;
        winWithEffect = 0;
    }
}
