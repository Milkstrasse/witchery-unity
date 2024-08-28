using UnityEngine;


[CreateAssetMenu(fileName = "Fighter", menuName = "ScriptableObjects/Fighter")]
public class Fighter : ScriptableObject
{
    public int fighterID;
    public Move[] moves;
}
