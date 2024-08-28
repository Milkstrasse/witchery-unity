using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public int fullHealth;
    public int currHealth;
    public int energy;
    public int[] fighterIDs;

    public List<Card> cards;
    public List<Card> cardHand;

    public List<StatusEffect> effects;

    public Action OnPlayerChanged;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SetupPlayer(PlayerMessage message)
    {
        playerName = message.name;
        fullHealth = 50;
        currHealth = fullHealth;
        energy = 7;

        cards = new List<Card>();
        cardHand = new List<Card>();
        effects = new List<StatusEffect>();

        fighterIDs = message.fighterIDs;

        for (int i = 0; i < fighterIDs.Length; i++)
        {
            Fighter fighter = GlobalManager.singleton.fighters[fighterIDs[i]];
            for (int j = 0; j < fighter.moves.Length; j++)
            {
                cards.Add(new Card(fighter, j));
            }
        }

        for (int i = 0; i < message.cardHand.Length; i++)
        {
            cardHand.Add(cards[message.cardHand[i]]);
        }      
    }

    public void UpdatePlayer(PlayerMessage message, bool updateCards)
    {
        currHealth = message.health;
        energy = message.energy;

        if (updateCards)
        {
            cardHand = new List<Card>();
            for (int i = 0; i < message.cardHand.Length; i++)
            {
                cardHand.Add(cards[message.cardHand[i]]);
            }
        }

        OnPlayerChanged?.Invoke();
    }
}