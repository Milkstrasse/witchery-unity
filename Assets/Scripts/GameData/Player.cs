using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerID;
    public int icon;

    public string playerName;
    public int fullHealth;
    public int currHealth;
    public int energy;
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
        icon = message.icon;
        
        playerName = message.name;
        fullHealth = GlobalData.health;
        currHealth = fullHealth;

        energy = 0;

        cards = new List<Card>();
        cardHand = new List<Card>();
        effects = message.effects.ToList();

        fighterIDs = message.fighterIDs;

        for (int i = 0; i < 5; i++) //blank cards
        {
            cards.Add(new Card());
        }

        for (int i = 0; i < fighterIDs.Length; i++)
        {
            Fighter fighter = GlobalManager.singleton.fighters[fighterIDs[i].fighterID];
            for (int j = 0; j < fighter.moves.Length; j++)
            {
                cards.Add(new Card(fighter, fighterIDs[i].outfit, j));
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
        effects = playerData.effects;

        if (updateCards)
        {
            cardHand = new List<Card>();
            for (int i = 0; i < playerData.cardHand.Count; i++)
            {
                cardHand.Add(cards[playerData.cardHand[i]]);
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
                effects[i].isNew = true;
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
            if (effects[i].name == "shields" || effects[i].name == "vulnerable" )
            {
                effects[i].isNew = true;
                modifier += effects[i].value * effects[i].multiplier;
            }
        }
        
        return modifier;
    }
}