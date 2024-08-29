using System.Collections.Generic;

public class PlayerData
{
    public string name;
    public int health;
    public int energy;
    public List<int> cardStack;
    public List<int> playedCards;
    public List<int> cardHand;
    public List<StatusEffect> effects;

    public PlayerData()
    {
        name = "";
        
        health = 0;
        energy = 0;

        cardStack = new List<int>();
        playedCards = new List<int>();
        cardHand = new List<int>();
        effects = new List<StatusEffect>();
    }

    public PlayerData(string name, List<int> cards)
    {
        this.name = name;
        
        health = 50;
        energy = 7;

        cardStack = cards;
        playedCards = new List<int>();
        cardHand = new List<int>();
        effects = new List<StatusEffect>();

        FillHand(5);
    }

    public void FillHand(int cardAmount)
    {
        int cardsToRemove = cardAmount;
        while (cardsToRemove > 0)
        {
            if (cardStack.Count == 0)
            {
                cardStack = playedCards;
                playedCards = new List<int>();
            }

            int randomIndex = UnityEngine.Random.Range(0, cardStack.Count);
            cardHand.Add(cardStack[randomIndex]);
            cardStack.RemoveAt(randomIndex);

            cardsToRemove--;
        }
    }

    public void RemoveCard(int cardIndex)
    {
        int card = cardHand[cardIndex];

        playedCards.Add(card);
        cardHand.RemoveAt(cardIndex);
    }
}
