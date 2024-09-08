using UnityEngine;


[CreateAssetMenu(fileName = "Move", menuName = "ScriptableObjects/Move")]
public class Move : ScriptableObject
{
    public int moveID;
    public int cost;
    public int[] health;
    public int[] energy;
    public StatusEffect[] effects;
    public MoveType moveType;

    public enum MoveType
    {
        Standard, Combo, Response
    }
}
