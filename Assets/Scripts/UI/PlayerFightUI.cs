using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFightUI : MonoBehaviour
{
    public Player player;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI energyText;

    [SerializeField] private Transform statusParent;
    private StatusUI[] effects;
    [SerializeField] private Transform cardParent;
    private CanvasGroup cardGroup;
    private Canvas canvas;

    private CardUI[] cards;

    private void Start()
    {
        cardGroup = cardParent.GetComponent<CanvasGroup>();
    }

    public void SetupUI(Canvas canvas, Player player)
    {
        this.player = player;
        player.OnPlayerChanged += UpdateUI;

        this.canvas = canvas;

        nameText.text = player.playerName;
        healthText.text = $"{player.currHealth}/{player.fullHealth}HP";
        energyText.text = player.energy.ToString();

        cards = new CardUI[5];
        effects = new StatusUI[5];

        float cardSpacer = (Screen.width/canvas.scaleFactor - 5 * 230 - 40)/4 * -1;

        for (int i = 0; i < 5; i++)
        {
            CardUI card = cardParent.transform.GetChild(i).GetComponent<CardUI>();
            card.transform.localPosition = new Vector3(i * (230 - cardSpacer), -160f, 0);
            card.GetComponent<DragDrop>().SetInit(i, this);

            cards[i] = card;

            if (i < player.cardHand.Count)
            {
                cards[i].SetupCard(player.cardHand[i]);
                cards[i].ShowCard(true);
            }
            else
            {
                cards[i].ShowCard(false);
            }

            effects[i] = statusParent.GetChild(i).GetComponent<StatusUI>();

            if (i < player.effects.Count)
            {
                effects[i].SetupEffect(player.effects[i]);
                effects[i].gameObject.SetActive(true);
            }
            else
            {
                effects[i].gameObject.SetActive(false);
            }
        }

        cardGroup.interactable = true;
    }

    private void UpdateUI()
    {
        healthText.text = $"{player.currHealth}/{player.fullHealth}HP";
        LeanTween.value(healthBar.gameObject, healthBar.value, player.currHealth/(float)player.fullHealth, 0.3f).setOnUpdate( (float val) => { healthBar.value = val; } );

        energyText.text = player.energy.ToString();

        for (int i = 0; i < 5; i++)
        {
            if (i < player.cardHand.Count)
            {
                cards[i].SetupCard(player.cardHand[i]);
                cards[i].ShowCard(true);
            }
            else
            {
                cards[i].ShowCard(false);
            }

            if (i < player.effects.Count)
            {
                effects[i].SetupEffect(player.effects[i]);
                effects[i].gameObject.SetActive(true);
            }
            else
            {
                effects[i].gameObject.SetActive(false);
            }
        }
    }

    public void MakeMove(MoveMessage message, CardSlot cardSlot)
    {
        if (message.playCard)
        {
            StartCoroutine(MoveCard(message.cardIndex, cardSlot));
        }
        else
        {
            StartCoroutine(RemoveCard(message.cardIndex));
        }
    }

    IEnumerator MoveCard(int cardIndex, CardSlot cardSlot)
    {
        cards[cardIndex].transform.SetParent(canvas.transform);
        
        LeanTween.move(cards[cardIndex].gameObject, new Vector3(cardSlot.transform.position.x + 172.5f, cardSlot.transform.position.y, cardSlot.transform.position.z), 0.5f);
        
        yield return new WaitForSeconds(0.5f);

        cards[cardIndex].GetComponent<DragDrop>().ResetDrag();
        cardSlot.SetupCard(player.cardHand[cardIndex], true);

        player.cardHand.RemoveAt(cardIndex);

        UpdateUI();
    }

    IEnumerator RemoveCard(int cardIndex)
    {
        yield return null;

        cards[cardIndex].GetComponent<DragDrop>().ResetDrag();
        player.cardHand.RemoveAt(cardIndex);

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnPlayerChanged -= UpdateUI;
        }
    }
}
