public class PlayedCard
{
    public Card card;
    public int player;
    public bool played;
    public int lastCost;

    public PlayedCard()
    {
        card = new Card();
        player = -1;
        played = true;
        lastCost = 0;
    }

    public PlayedCard(Card card, int player, bool played, PlayedCard lastCard)
    {
        this.card = card;
        this.player = player;
        this.played = played;

        lastCost = lastCard.card.hasMove ? lastCard.card.move.cost : 0;
    }
}