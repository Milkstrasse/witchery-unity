using System;

[Serializable]
public class StatusEffect
{
    public string icon;
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
        icon = "";
        duration = 0;
        value = 0;
        statusType = StatusType.Health;
        isDelayed = false;
    }

    public StatusEffect(StatusEffect initEffect)
    {
        icon = initEffect.icon;
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