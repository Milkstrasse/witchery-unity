using System.Collections.Generic;

public class PlayerData
{
    public string name;
    public int health;
    public int energy;
    public List<int> cardStack;
    public List<int> playedCards;
    public List<int> cardHand;

    public PlayerData(string name, List<int> cards)
    {
        this.name = name;
        
        health = 50;
        energy = 7;

        cardStack = cards;
        playedCards = new List<int>();
        cardHand = new List<int>();

        int cardsToRemove = 5;
        while (cardsToRemove > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, cardStack.Count);
            cardHand.Add(cardStack[randomIndex]);
            cardStack.RemoveAt(randomIndex);
            
            cardsToRemove--;
        }
    }
}
