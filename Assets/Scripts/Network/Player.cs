using Mirror;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class Player : NetworkBehaviour
{
    public static Player localPlayer;

    [SyncVar] public string teamName;
    public int fullHealth;
    [SyncVar (hook = nameof(UpdateHealth))] public int currHealth;

    public Fighter[] fighters;

    public readonly SyncList<Card> cardHand = new SyncList<Card>();
    public List<Card> playedCards;
    public List<Card> stackCards;

    [SyncVar] public int energy;
    public readonly SyncList<StatusEffect> effects = new SyncList<StatusEffect>();

    public Action<int> onTurnChange;
    public Action onPlayerChanged;

    public bool gaveUp;

    public override void OnStartClient()
    {
        cardHand.OnChange += OnCardsUpdated;
        effects.OnChange += OnEffectsUpdated;
        fullHealth = 50;
    }

    void Start()
    {
        if (isOwned)
        {
            localPlayer = this;
        }
        
        DontDestroyOnLoad(this);
        NetworkClient.ReplaceHandler<TurnMessage>(OnReceiveMessage);
    }

    [Server]
    public void SetupPlayer(TeamMessage message)
    {
        teamName = message.name;
        fullHealth = 50;
        currHealth = 50;
        playedCards = new List<Card>();
        stackCards = new List<Card>();

        fighters = new Fighter[message.fighterIDs.Length];
        for (int i = 0; i < message.fighterIDs.Length; i++)
        {
            fighters[i] = GlobalManager.singleton.fighters[message.fighterIDs[i]];
            for (int j = 0; j < fighters[i].moves.Length; j++)
            {
                stackCards.Add(new Card(fighters[i], j));
            }
        }

        ShuffleCards(stackCards);
        FillHand(5);

        energy = 7;
    }

    private void ShuffleCards(List<Card> cards)
    {
		var count = cards.Count;
		var last = count - 1;

		for (var i = 0; i < last; ++i)
        {
			int index = UnityEngine.Random.Range(i, count);
            (cards[index], cards[i]) = (cards[i], cards[index]);
        }
    }

    public void NewRound()
    {
        energy += 7;
        FillHand(5);

        int i = 0;
        while (i < effects.Count)
        {
            effects[i].duration -= 1; //does not trigger change

            switch (effects[i].name)
            {
                case "Bleed":
                    currHealth = Math.Clamp(currHealth - 5, 0, fullHealth);
                    break;
                case "Bomb":
                    if (effects[i].duration == 1)
                        currHealth = Math.Clamp(currHealth - 20, 0, fullHealth);

                    break;
                case "Heal":
                    currHealth = Math.Clamp(currHealth + 5, 0, fullHealth);
                    break;
                default:
                    break;
            }

            //triggers change
            if (effects[i].duration <= 0)
            {
                effects.RemoveAt(i);
            }
            else
            {
                effects[i] = new StatusEffect(effects[i].name, effects[i].duration);
            }

            i++;
        }

        SetEnergy(energy);
    }

    [ClientRpc]
    private void SetEnergy(int energy)
    {
        this.energy = energy;
    }

    private void FillHand(int amount)
    {
        int cardAmount = Math.Min(amount, stackCards.Count + playedCards.Count);
        while (cardAmount > 0)
        {
            if (stackCards.Count == 0)
            {
                ShuffleCards(playedCards);
                stackCards = playedCards;
                playedCards = new List<Card>();
            }

            cardHand.Add(stackCards[0]);
            stackCards.RemoveAt(0);
            cardAmount--;
        }
    }

    public bool HasResponse(Move move)
    {
        if (cardHand.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < cardHand.Count; i++)
        {
            if (cardHand[i].moveType == MoveType.Response)
            {
                return true;
            }
        }

        return false;
    }

    private void OnReceiveMessage(TurnMessage message)
    {
        Debug.Log("RECEIVED TURN MESSAGE");
        StartCoroutine(ChangeTurn(message.turn));
    }

    IEnumerator ChangeTurn(int turn)
    {
        while (onTurnChange == null || onTurnChange.GetInvocationList().Length == 0)
        {
            yield return null;
        }

        onTurnChange.Invoke(turn);
    }

    public void MakeMove(int _cardIndex, bool _play, bool _toRemove)
    {
        MoveMessage msg = new MoveMessage {
            cardIndex = _cardIndex,
            cardPlayed = _play,
            toRemove = _toRemove
        };

        NetworkClient.Send(msg);
    }

    private void OnCardsUpdated(SyncList<Card>.Operation op, int index, Card card)
    {
        onPlayerChanged?.Invoke();
    }

    private void OnEffectsUpdated(SyncList<StatusEffect>.Operation op, int index, StatusEffect effect)
    {
        onPlayerChanged?.Invoke();
    }

    public void UpdateHealth(int oldValue, int newValue)
    {
        onPlayerChanged?.Invoke();
    }

    [ClientRpc]
    public void RpcOnCardsUpdated()
    {
        onPlayerChanged?.Invoke();
    }

    [Command]
    public void GiveUp()
    {
        gaveUp = true;
        NetworkManager.singleton.ServerChangeScene("GameOverScene");
    }
}
