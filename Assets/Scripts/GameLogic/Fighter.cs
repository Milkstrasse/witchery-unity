using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Fighter", menuName = "ScriptableObjects/Fighter")]
public class Fighter : ScriptableObject
{
    public int fighterID;
    public Role role;
    public Move[] moves;
}

public enum Role: uint
{
    attack = 62796,
    support = 62445
}