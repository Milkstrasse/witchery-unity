using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Fighter", menuName = "ScriptableObjects/Fighter")]
public class Fighter : ScriptableObject
{
    public int fighterID;
    public Role role;
    public Move[] moves;
    public Outfit[] outfits; //has to be same amount! for all fighters because uf 2d array, [][] jagged array could be alternative
}

public enum Role: uint //use decimal from hexadecimal to decimal converter
{
    attack = 62796,
    support = 62445
}