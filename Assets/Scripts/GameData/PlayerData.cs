using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public bool startedFirst;
    public int roundsPlayed;
    public bool playedUntilEnd;
    public int maxEffects;
    public bool healedOpponent;
    public bool stoleNothing;
    public bool wonWithEffect;
    public bool selfKO;

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

        health = message.health;
        energy = 0;

        cardStack = new List<int>();
        for (int i = 0; i < message.fighterIDs.Length; i++)
        {
            int currCount = cardStack.Count + 5;

            Fighter fighter = GlobalData.fighters[message.fighterIDs[i].fighterID];
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
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].name == effect.name)
            {
                index = i;
                break;
            }
        }

        if (index >= 0 && effects[index].multiplier < GlobalData.effectLimit)
        {
            effects[index].multiplier += effect.multiplier;
            effects[index].isNew = true;
        }
        else if (index < 0 && effects.Count < 5)
        {
            effects.Add(effect);

            maxEffects = Math.Max(effects.Count, maxEffects);
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
                effects[i].isNew = true;
                power += effects[i].value * effects[i].multiplier;
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
                modifier += effects[i].value * effects[i].multiplier;
            }
        }
        
        return modifier;
    }

    public StatusEffect GetEffect(string effectName, bool setNew = true)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].name == effectName)
            {
                effects[i].isNew = setNew;
                return effects[i];
            }
        }

        return null;
    }

    public int CheckEffectBalance()
    {
        int balance = 0;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].value >= 0)
            {
                balance += effects[i].multiplier;
            }
            else
            {
                balance -= effects[i].multiplier;
            }
        }

        return balance;
    }

    public void AddBlanks(int amount)
    {
        int prevIndex = startIndex;
        startIndex = Math.Max(startIndex - amount, 0);

        if (prevIndex != startIndex)
        {
            while (amount > 0)
            {
                cardStack.Add(startIndex);
                amount--;
            }
            
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

        bool shuffle = false;

        for (int i = 0; i < cardStack.Count; i++)
        {
            if (toRemove == 0)
            {
                return;
            }

            if (cardStack[i] < startIndex)
            {
                cardStack.RemoveAt(i);
                shuffle = true;
                toRemove--;
            }
        }

        if (shuffle)
        {
            ShuffleStack();
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
