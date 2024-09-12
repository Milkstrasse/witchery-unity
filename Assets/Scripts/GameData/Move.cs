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

    public enum MoveType
    {
        Standard, Response
    }

    public string GetDescription(int offset = 0)
    {
        switch (moveID - offset)
        {
            case 0:
                return "doDamage";
            case 1:
                return "recoverHP";
            case 2:
                return "gainEnergy";
            case 3:
                return "gainEffect";
            case 4:
            case 5:
                return "giveEffect";
            case 6:
                return "stealEnergy";
            case 10:
                return "doCostDamage";
            case 11:
                return "recoverCostHP";
            case 12:
                return "gainCostEnergy";
            case 20:
                return "stealHP";
            default:
                return name;
        }
    }
}
