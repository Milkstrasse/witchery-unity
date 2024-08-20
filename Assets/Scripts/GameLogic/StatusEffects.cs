using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public string name;
    public int duration;

    public StatusEffect()
    {
        name = "";
        duration = 0;
    }

    public StatusEffect(string name, int duration)
    {
        this.name = name;
        this.duration = duration;
    }

    public static StatusEffect GetStatus(StatusEffects statusEffect)
    {
        return statusEffect switch
        {
            StatusEffects.Heal => new StatusEffect(statusEffect.ToString(), 3),
            StatusEffects.Bleed => new StatusEffect(statusEffect.ToString(), 3),
            StatusEffects.Bomb => new StatusEffect(statusEffect.ToString(), 3),
            _ => new StatusEffect("", 0)
        };
    }

}

public enum StatusEffects
{
    Empty, Heal, Bleed, Bomb

}