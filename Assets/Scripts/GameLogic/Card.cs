using System;

[Serializable]
public struct Card
{
    public int fighterID;
    public int moveIndex;
    public int cost;
    public MoveType moveType;

    public Card(Fighter fighter, int moveIndex)
    {
        fighterID = fighter.fighterID;
        this.moveIndex = moveIndex;

        Move move = GlobalManager.singleton.fighters[fighterID].moves[moveIndex];
        cost = move.cost;
        moveType = move.moveType;
    }
}
