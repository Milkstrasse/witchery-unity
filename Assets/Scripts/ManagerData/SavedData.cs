using System;
using UnityEngine;

[Serializable]
public struct SavedData
{
    public string name;
    public int icon;
    public int money;
    public bool[,] unlocked;
    public SelectedFighter[] shopFighters;

    public SavedData(string name, int icon, int money, bool[,] unlocked, SelectedFighter[] shopFighters)
    {
        this.name = name;
        this.icon = icon;
        this.money = money;
        this.unlocked = unlocked;
        this.shopFighters = shopFighters;
    }
}
