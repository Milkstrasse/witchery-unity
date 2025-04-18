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

    public string GetDescription(bool ignoreType)
    {
        if (moveType == MoveType.Response)
        {
            return name;
        }

        switch (moveID)
        {
            case 2:
                if (!ignoreType && moveType == MoveType.Special)
                {
                    return "doCostDamage";
                }
                else
                {
                    return "doDamage";
                }
            case 3:
                if (!ignoreType && moveType == MoveType.Special)
                {
                    return "recoverCostHP";
                }
                else
                {
                    return "recoverHP";
                }
            case 4:
                if (!ignoreType && moveType == MoveType.Special)
                {
                    return "stealCostHP";
                }
                else
                {
                    return "stealHP";
                }
            case 8:
                if (!ignoreType && moveType == MoveType.Special)
                {
                    return "stealCostEnergy";
                }
                else
                {
                    return "stealEnergy";
                }
            case 9:
                if (!ignoreType && moveType == MoveType.Special)
                {
                    return "gainCostEnergy";
                }
                else
                {
                    return "gainEnergy";
                }
            case 10:
                return "doSpecialDamage";
            case 14:
                if (!ignoreType && moveType == MoveType.Special)
                {
                    return "applyCostEffect";
                }
                else
                {
                    return "applyEffect";
                }
            case 15:
                if (!ignoreType && moveType == MoveType.Special)
                {
                    return "obtainCostEffect";
                }
                else
                {
                    return "obtainEffect";
                }
            case 21:
                return "healToHP";
            default:
                return name;
        }
    }

    public bool IsResponseTo(Move move, int playerEnergy)
    {
        if (move == null || moveType != MoveType.Response || cost > playerEnergy)
        {
            return false;
        }

        return move.moveID % moveID == 0;
    }
}

public enum MoveType
{
    Standard, Response, Special
}