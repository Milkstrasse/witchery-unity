using UnityEngine;


[CreateAssetMenu(fileName = "Move", menuName = "ScriptableObjects/Move")]
public class Move : ScriptableObject
{
    public int moveID;
    public int cost;
    public int target;
    public int health;
    public int energy;
    public StatusEffect effect;
    public MoveType moveType;

    public string GetDescription(int offset = 0)
    {
        if (moveType != MoveType.Standard)
        {
            return name;
        }
        
        switch (moveID - offset)
        {
            case 3:
                return "recoverHP";
            case 4:
                return "doDamage";
            case 6:
                return "healToHP";
            case 8:
                return "giveEffect";
            case 9:
                return "gainEnergy";
            case 15:
                return "recoverCostHP";
            case 16:
                return "doCostDamage";
            case 18:
                return "gainEffect";
            case 20:
                return "stealEnergy";
            case 21:
                return "gainCostEnergy";
            case 28:
                return "stealHP";
            default:
                return name;
        }
    }
}

public enum MoveType
{
    Standard, Response
}