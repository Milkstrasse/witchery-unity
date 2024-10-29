using System;
using UnityEngine;

[Serializable]
public struct SavedData
{
    public string name;
    public int icon;
    public int money;
    public bool[,] unlocked;

    public SavedData(string name, int icon, int money, bool[,] unlocked)
    {
        this.name = name;
        this.icon = icon;
        this.money = money;
        this.unlocked = unlocked;
    }
}
