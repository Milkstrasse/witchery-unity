using System;

[Serializable]
public class StatusEffect
{
    public string name;
    public string icon;
    public int duration;
    public int value;
    public StatusType statusType;
    public bool isDelayed;

    public bool isNew;

    public enum StatusType
    {
        Health, Energy, Power, Special
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
        name = initEffect.name;
        icon = initEffect.icon;
        duration = initEffect.duration;
        value = initEffect.value;
        statusType = initEffect.statusType;
        isDelayed = initEffect.isDelayed;

        isNew = true;
    }

    public void TriggerEffect(PlayerData player)
    {
        switch (statusType)
        {
            case StatusType.Energy:
                player.energy += value;
                isNew = true;
                
                break;
            case StatusType.Power:
                break;
            case StatusType.Health:
                if (!isDelayed || (isDelayed && duration == 1))
                {
                    player.health = Math.Clamp(player.health + value, 0, 50);
                    isNew = true;
                }
                break;
            default: //StatusType.Special
                break;
        }
    }
}