using System;
using UnityEngine;

[Serializable]
public struct SavedData
{
    public string name;
    public int money;
    public bool[,] unlocked;

    public SavedData(string name, int money, bool[,] unlocked)
    {
        this.name = name;
        this.money = money;
        this.unlocked = unlocked;
    }
}
