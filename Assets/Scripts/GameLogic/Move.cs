using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "ScriptableObjects/Move")]
public class Move : ScriptableObject
{
    public int moveID;
    public MoveType moveType;
    public int cost;
    public int[] health;
    public int[] energy;
    public StatusEffects[] statusEffects;
}
