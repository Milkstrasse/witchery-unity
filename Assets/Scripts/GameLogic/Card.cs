using System;

[Serializable]
public struct Card
{
    public int fighterID;
    public int moveID;
    public int cost;

    public Card(Fighter fighter, int moveID)
    {
        fighterID = fighter.fighterID;
        this.moveID = moveID;

        cost = GlobalManager.singleton.fighters[fighterID].moves[moveID].cost;
    }
}
