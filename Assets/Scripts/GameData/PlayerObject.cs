using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    public int playerID;
    public int icon;

    public int fullHealth;
    public int currHealth;
    public int energy;
    public int blanks;
    public SelectedFighter[] fighterIDs;

    public List<Card> cards;
    public List<Card> cardHand;

    public List<StatusEffect> effects;

    public bool hasWon;

    public Action OnPlayerChanged;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SetupPlayer(PlayerMessage message, int playerID)
    {
        this.playerID = playerID;
        
        fullHealth = message.health;
        currHealth = message.health;

        energy = 0;
        blanks = 0;

        cards = new List<Card>();
        cardHand = new List<Card>();
        effects = message.effects.ToList();

        fighterIDs = message.fighterIDs;

        for (int i = 0; i < fighterIDs.Length; i++)
        {
            Fighter fighter = GlobalData.fighters[fighterIDs[i].fighterID];
            if (i == 0)
            {
                for (int j = 0; j < fighter.moves.Length; j++)
                {
                    cards.Add(new Card(fighter, fighterIDs[i].outfit, j));
                }
            }
            else
            {
                cards.Add(new Card(fighter, fighterIDs[i].outfit, 0));
            }
        }

        for (int i = 0; i < message.cardHand.Length; i++)
        {
            cardHand.Add(cards[message.cardHand[i]]);
        }      
    }

    public void UpdatePlayer(PlayerData playerData, bool updateCards)
    {
        currHealth = playerData.health;
        energy = playerData.energy;
        blanks = playerData.blanks;
        effects = playerData.effects;

        if (updateCards)
        {
            cardHand = new List<Card>();
            for (int i = 0; i < playerData.cardHand.Count; i++)
            {
                if (playerData.cardHand[i] < 0)
                {
                    cardHand.Add(new Card());
                }
                else
                {
                    cardHand.Add(cards[playerData.cardHand[i]]);
                }
            }
        }

        OnPlayerChanged?.Invoke();
    }

    public bool HasResponseTo(Move move)
    {
        for (int i = 0; i < cardHand.Count; i++)
        {
            if (!cardHand[i].hasMove)
            {
                continue;
            }

            if (cardHand[i].move.IsResponseTo(move, energy))
            {
                return true;
            }
        }

        return false;
    }

    public int GetPowerBonus()
    {
        int power = 0;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].statusType == StatusEffect.StatusType.Power)
            {
                power += effects[i].value * effects[i].multiplier;
            }
        }

        return power;
    }

    public int GetBlanksInHand()
    {
        int counter = 0;
        for (int i = 0; i < cardHand.Count; i++)
        {
            if (!cardHand[i].hasMove)
            {
                counter++;
            }
        }

        return counter;
    }

    public int GetDamageModifier()
    {
        int modifier = 0;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].statusType == StatusEffect.StatusType.Damage)
            {
                modifier += effects[i].value * effects[i].multiplier;
            }
        }
        
        return modifier;
    }
}