using System;

[Serializable]
public class StatusEffect
{
    public int duration;
    public int value;
    public StatusType statusType;
    public bool isDelayed;

    public enum StatusType
    {
        Health, Energy, Power
    }

    public StatusEffect()
    {
        duration = 0;
        value = 0;
        statusType = StatusType.Health;
        isDelayed = false;
    }

    public StatusEffect(StatusEffect initEffect)
    {
        duration = initEffect.duration;
        value = initEffect.value;
        statusType = initEffect.statusType;
        isDelayed = initEffect.isDelayed;
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