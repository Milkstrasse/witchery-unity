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

    public void TriggerEffect(PlayerData player)
    {
        switch (statusType)
        {
            case StatusType.Energy:
                player.energy += value;
                break;
            case StatusType.Power:
                break;
            default: //StatusType.Health
                if (!isDelayed || (isDelayed && duration == 1))
                {
                    player.health = Math.Clamp(player.health + value, 0, 50);
                }

                break;
        }
    }
}