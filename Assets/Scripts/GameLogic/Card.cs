using System;

[Serializable]
public struct Card
{
    public int fighterID;
    public int moveID;
    public int cost;
    public MoveType moveType;

    public Card(Fighter fighter, int moveID)
    {
        fighterID = fighter.fighterID;
        this.moveID = moveID;

        Move move = GlobalManager.singleton.fighters[fighterID].moves[moveID];
        cost = move.cost;
        moveType = move.moveType;
    }
}
