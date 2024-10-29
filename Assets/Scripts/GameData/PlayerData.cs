using System;
using System.Collections.Generic;
using System.Linq;

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
        
        health = GlobalSettings.health;
        energy = 7;

        cardStack = new List<int>();
        for (int i = 0; i < message.fighterIDs.Length; i++)
        {
            int currCount = cardStack.Count + 5;

            Fighter fighter = GlobalManager.singleton.fighters[message.fighterIDs[i].fighterID];
            for (int j = 0; j < fighter.moves.Length; j++)
            {
                cardStack.Add(currCount + j);
            }
        }

        playedCards = new List<int>();
        cardHand = new List<int>();
        effects = message.effects.ToList();

        startIndex = 5;

        ShuffleStack();

        FillHand(5);
    }

    private void ShuffleStack()
    {
        int n = cardStack.Count - 1;
        while (n > 0)
        {
            int j = UnityEngine.Random.Range(0, n);
            int tmp = cardStack[n];
            cardStack[n] = cardStack[j];
            cardStack[j] = tmp;

            n--;
        }
    }

    public void FillHand(int cardAmount)
    {
        int amount = Math.Min(cardAmount, playedCards.Count + cardStack.Count);

        int i = 0;
        while (i < amount)
        {
            if (cardStack.Count == 0)
            {
                cardStack = playedCards;
                playedCards = new List<int>();

                ShuffleStack();
            }

            cardHand.Add(cardStack[0]);
            cardStack.RemoveAt(0);

            i++;
        }
    }

    public void RemoveCard(int cardIndex)
    {
        int card = cardHand[cardIndex];
        cardHand.RemoveAt(cardIndex);
        playedCards.Add(card);
    }

    public void AddEffect(StatusEffect effect)
    {
        int index = -1;
        if (!effect.isDelayed)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].name == effect.name)
                {
                    index = i;
                    break;
                }
            }
        }

        if (index >= 0)
        {
            effects[index].duration += effect.duration;
            effects[index].isNew = true;
        }
        else if (effects.Count < 5)
        {
            effects.Add(effect);
        }
    }

    public void UnmarkEffects()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].isNew = false;
        }
    }

    public int GetPowerBonus()
    {
        int power = 0;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].statusType == StatusEffect.StatusType.Power)
            {
                power += effects[i].value;
                effects[i].isNew = true;
            }
        }

        return power;
    }

    public int GetDamageModifier()
    {
        int modifier = 0;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].name == "shields" || effects[i].name == "vulnerable" )
            {
                effects[i].isNew = true;
                modifier += effects[i].value;
            }
        }
        
        return modifier;
    }

    public StatusEffect GetEffect(string effectName)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].name == effectName)
            {
                effects[i].isNew = true;
                return effects[i];
            }
        }

        return null;
    }

    public void AddBlank()
    {
        int prevIndex = startIndex;
        startIndex = Math.Max(startIndex - 1, 0);

        if (prevIndex != startIndex)
        {
            cardStack.Add(startIndex);
            ShuffleStack();
        }
    }

    public void RemoveBlanks()
    {
        int toRemove = 5 - startIndex;

        for (int i = 0; i < cardHand.Count; i++)
        {
            if (toRemove == 0)
            {
                return;
            }

            if (cardHand[i] < startIndex)
            {
                cardHand.RemoveAt(i);

                toRemove--;
            }
        }

        for (int i = 0; i < cardStack.Count; i++)
        {
            if (toRemove == 0)
            {
                return;
            }

            if (cardStack[i] < startIndex)
            {
                cardStack.RemoveAt(i);
                toRemove--;
            }
        }

        for (int i = 0; i < playedCards.Count; i++)
        {
            if (toRemove == 0)
            {
                return;
            }

            if (playedCards[i] < startIndex)
            {
                playedCards.RemoveAt(i);
                toRemove--;
            }
        }
    }
}
