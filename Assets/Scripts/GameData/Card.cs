public struct Card
{
    public Fighter fighter;
    public Move move;
    public bool hasMove;

    public Card(Fighter fighter, int moveIndex)
    {
        this.fighter = fighter;
        move = fighter.moves[moveIndex];
        hasMove = true;
    }
}