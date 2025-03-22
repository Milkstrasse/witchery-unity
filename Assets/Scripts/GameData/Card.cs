public struct Card
{
    public Fighter fighter;
    public int outfit;
    public Move move;
    public bool hasMove;
    public bool isSpecial;

    public Card(Fighter fighter, int outfit, int moveIndex)
    {
        this.fighter = fighter;
        this.outfit = outfit;
        move = fighter.moves[moveIndex];
        hasMove = true;

        isSpecial = moveIndex == 0;
    }

    public Card(Card card)
    {
        fighter = card.fighter;
        outfit = card.outfit;
        move = card.move;
        hasMove = card.hasMove;
        isSpecial = card.isSpecial;
    }
}