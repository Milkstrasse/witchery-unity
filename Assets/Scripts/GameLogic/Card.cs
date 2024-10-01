public struct Card
{
    public Fighter fighter;
    public int outfit;
    public Move move;
    public bool hasMove;

    public Card(Fighter fighter, int outfit, int moveIndex)
    {
        this.fighter = fighter;
        this.outfit = outfit;
        move = fighter.moves[moveIndex];
        hasMove = true;
    }
}