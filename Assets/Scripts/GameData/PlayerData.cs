using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerData
{
    public string name;
    public int health;
    public int maxHealth;
    public int energy;
    public List<int> cardStack;
    public List<int> playedCards;
    public List<int> cardHand;
    public List<StatusEffect> effects;

    public int blanks;

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
        maxHealth = 0;
        energy = 0;

        cardStack = new List<int>();
        playedCards = new List<int>();
        cardHand = new List<int>();
        effects = new List<StatusEffect>();

        blanks = 0;
    }

    public PlayerData(PlayerMessage message)
    {
        health = message.health;
        maxHealth = message.health;
        energy = 0;

        cardStack = new List<int>();
        for (int i = 0; i < message.fighterIDs.Length; i++)
        {
            int currCount = cardStack.Count;

            Fighter fighter = GlobalData.fighters[message.fighterIDs[i].fighterID];
            if (i == 0)
            {
                for (int j = 0; j < fighter.moves.Length; j++)
                {
                    cardStack.Add(currCount + j);
                }
            }
            else
            {
                cardStack.Add(currCount);
            }
        }

        playedCards = new List<int>();
        cardHand = new List<int>();
        effects = message.effects.ToList();

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

        if (index >= 0 && effects[index].multiplier < GlobalData.stackLimit)
        {
            int multiplier = effects[index].multiplier + effect.multiplier;

            effects[index].multiplier = Math.Min(multiplier, GlobalData.stackLimit);
            effects[index].isNew = true;
        }
        else if (index < 0 && effects.Count < GlobalData.effectLimit)
        {
            if (effect.multiplier > GlobalData.stackLimit)
            {
                effects.Add(new StatusEffect(effect, GlobalData.stackLimit));
            }
            else
            {
                effects.Add(effect);
            }

            maxEffects = Math.Max(effects.Count, maxEffects);
        }
    }

    public void UnmarkEffects()
    {
        int i = 0;
        while (i < effects.Count)
        {
            effects[i].isNew = false;
            
            if (effects[i].multiplier <= 0)
            {
                effects.RemoveAt(i);
            }
            else
            {
                i++;
            }
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

                ConsumeEffect(i);
            }
        }

        return power;
    }

    public int GetDamageModifier()
    {
        int modifier = 0;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].statusType == StatusEffect.StatusType.Damage)
            {
                effects[i].isNew = true;
                modifier += effects[i].value * effects[i].multiplier;

                ConsumeEffect(i);
            }
        }
        
        return modifier;
    }

    public int GetEffect(string effectName, bool setNew = true)
    {
        int value = 0;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].name == effectName)
            {
                effects[i].isNew = setNew;
                value = effects[i].value * effects[i].multiplier;

                if (setNew)
                {
                    ConsumeEffect(i);
                }

                return value;
            }
        }

        return value;
    }

    private void ConsumeEffect(int index)
    {
        effects[index].multiplier -= 1;
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
        int initBlanks = blanks;
        blanks = Math.Min(blanks + amount, GlobalData.blankLimit);

        int added = blanks - initBlanks;

        for (int i = 0; i < added; i++)
        {
            cardStack.Add(-1);
        }

        if (added > 0)
        {
            ShuffleStack();
        }
    }

    public void RemoveBlanks()
    {
        for (int i = 0; i < cardHand.Count; i++)
        {
            if (blanks == 0)
            {
                return;
            }

            if (cardHand[i] < 0)
            {
                cardHand.RemoveAt(i);
                blanks--;
            }
        }

        bool shuffle = false;

        for (int i = 0; i < cardStack.Count; i++)
        {
            if (blanks == 0)
            {
                return;
            }

            if (cardStack[i] < 0)
            {
                cardStack.RemoveAt(i);
                shuffle = true;
                blanks--;
            }
        }

        if (shuffle)
        {
            ShuffleStack();
        }

        for (int i = 0; i < playedCards.Count; i++)
        {
            if (blanks == 0)
            {
                return;
            }

            if (playedCards[i] < 0)
            {
                playedCards.RemoveAt(i);
                blanks--;
            }
        }
    }
}
