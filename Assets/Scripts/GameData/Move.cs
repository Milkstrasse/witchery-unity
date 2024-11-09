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
            case 2:
                return "doDamage";
            case 3:
                return "recoverHP";
            case 4:
                return "stealHP";
            case 8:
                return "doCostDamage";
            case 9:
                return "recoverCostHP";
            case 10:
                return "stealCostHP";
            case 14:
                return "stealEnergy";
            case 15:
                return "gainEnergy";
            case 16:
                return "doSpecialDamage";
            case 20:
                return "stealCostEnergy";
            case 21:
                return "gainCostEnergy";
            case 26:
                return "applyEffect";
            case 27:
                return "obtainEffect";
            case 33:
                return "healToHP";
            default:
                return name;
        }
    }

    public bool IsResponseTo(Move move, int playerEnergy)
    {
        if (moveType != MoveType.Response)
        {
            return false;
        }
        else if (cost > playerEnergy)
        {
            return false;
        }


        return move.moveID%moveID == 0;
    }
}

public enum MoveType
{
    Standard, Response
}