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
}
