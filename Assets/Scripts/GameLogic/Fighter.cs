using UnityEngine;


[CreateAssetMenu(fileName = "Fighter", menuName = "ScriptableObjects/Fighter")]
public class Fighter : ScriptableObject
{
    public int fighterID;
    public Role role;
    public Move[] moves;
    public StatusEffect effect;
}

public enum Role
{
    attack, support
}