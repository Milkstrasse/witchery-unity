using System;

[Serializable]
public class StatusEffect
{
    public string name;
    public string icon;
    public int multiplier;
    public int value;
    public StatusType statusType;

    public bool isNew;

    public enum StatusType
    {
        Health, Energy, Power, Special
    }

    public StatusEffect()
    {
        icon = "";
        multiplier = 0;
        value = 0;
        statusType = StatusType.Health;
    }

    public StatusEffect(StatusEffect initEffect, int amount)
    {
        name = initEffect.name;
        icon = initEffect.icon;
        multiplier = amount;
        value = initEffect.value;
        statusType = initEffect.statusType;

        isNew = true;
    }

    public void TriggerEffect(PlayerData player)
    {
        switch (statusType)
        {
            case StatusType.Energy:
                player.energy += value * multiplier;

                isNew = true;
                multiplier -= 1;

                break;
            case StatusType.Power:
                break;
            case StatusType.Health:
                player.health = Math.Clamp(player.health + value * multiplier, 0, GlobalData.health);
                
                isNew = true;
                multiplier -= 1;

                break;
            default: //StatusType.Special
                break;
        }
    }
}