using System;

[Serializable]
public class StatusEffect
{
    public int effectID;
    public int duration;
    public int value;
    public StatusType statusType;
    public bool isDelayed;

    public enum StatusType
    {
        Health, Energy, Power
    }
}