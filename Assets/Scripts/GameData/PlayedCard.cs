
public class PlayedCard
{
    public Card card;
    public int player;
    public bool played;

    public PlayedCard()
    {
        card = new Card();
        player = -1;
        played = true;
    }

    public PlayedCard(Card card, int player, bool played)
    {
        this.card = card;
        this.player = player;
        this.played = played;
    }
}
