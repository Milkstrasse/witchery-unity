using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "ScriptableObjects/Move")]
public class Move : ScriptableObject
{
    public int moveType;
    public int cost;
    public int[] health;
    public StatusEffects[] statusEffects;
}
