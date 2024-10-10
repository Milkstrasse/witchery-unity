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

    public readonly bool IsSpecialMove => (move.moveID >= 8 && move.moveID <= 10) || (move.moveID >= 20 && move.moveID <= 21);
}