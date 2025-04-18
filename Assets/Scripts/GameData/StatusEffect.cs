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
        Health, Power, Damage, Special
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
            case StatusType.Health:
                player.currHealth = Math.Clamp(player.currHealth + value * multiplier, 0, player.fullHealth);

                isNew = true;
                multiplier -= 1;

                break;
            default:
                break;
        }
    }
}