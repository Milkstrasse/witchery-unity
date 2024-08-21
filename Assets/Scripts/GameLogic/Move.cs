using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "ScriptableObjects/Move")]
public class Move : ScriptableObject
{
    public int moveID;
    public MoveType moveType;
    public int cost;
    public int[] health = new int[2];
    public StatusEffects[] statusEffects = new StatusEffects[2];
}
