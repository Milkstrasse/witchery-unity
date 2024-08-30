using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerID;

    public string playerName;
    public int fullHealth;
    public int currHealth;
    public int energy;
    public int[] fighterIDs;

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
}