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

    public int startIndex;

    public PlayerData()
    {
        name = "";
        
        health = 0;
        energy = 0;

        cardStack = new List<int>();
        playedCards = new List<int>();
        cardHand = new List<int>();
        effects = new List<StatusEffect>();

        startIndex = 0;
    }

    public PlayerData(PlayerMessage message)
    {
        name = message.name;
        
        health = 50;
        energy = 7;

        cardStack = new List<int> { 0, 1, 2, 3, 4 };
        for (int i = 0; i < message.fighterIDs.Length; i++)
        {
            Fighter fighter = GlobalManager.singleton.fighters[message.fighterIDs[i]];
            for (int j = 0; j < fighter.moves.Length; j++)
            {
                cardStack.Add(i * fighter.moves.Length + j + 5);
            }
        }

        playedCards = new List<int>();
        cardHand = new List<int>();
        effects = new List<StatusEffect>();

        startIndex = 5;

        FillHand(5);
    }

    public void AddEffect(StatusEffect effect)
    {
        int index = -1;
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].icon == effect.icon)
            {
                index = i;
                break;
            }
        }

        if (index >= 0)
        {
            effects[index].duration += effect.duration;
        }
        else if (effects.Count < 5)
        {
            effects.Add(effect);
        }
    }

    public void FillHand(int cardAmount)
    {
        int cardsToRemove = cardAmount;
        while (cardsToRemove > 0)
        {
            if (cardStack.Count <= startIndex)
            {
                cardStack.AddRange(playedCards);
                playedCards = new List<int>();
            }

            int randomIndex = UnityEngine.Random.Range(startIndex, cardStack.Count);
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

    public int GetPowerBonus()
    {
        int power = 0;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].statusType == StatusEffect.StatusType.Power)
            {
                power += effects[i].value;
            }
        }

        return power;
    }
}
