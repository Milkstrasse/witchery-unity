using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFightUI : MonoBehaviour
{
    public Player player;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private Image timer;

    [SerializeField] private Transform statusParent;
    private StatusUI[] effects;
    [SerializeField] private Transform cardParent;
    private CanvasGroup cardGroup;
    [SerializeField] private Canvas canvas;

    private CardUI[] cards;

    private void Start()
    {
        cardGroup = cardParent.GetComponent<CanvasGroup>();

        cards = new CardUI[5];
        effects = new StatusUI[5];

        InitUI();
    }

    private void InitUI()
    {
        float cardSpacer = (Screen.width/canvas.scaleFactor - 5 * 230 - 40)/4 * -1;

        for (int i = 0; i < 5; i++)
        {
            CardUI card = cardParent.transform.GetChild(i).GetComponent<CardUI>();
            card.transform.localPosition = new Vector3(i * (230 - cardSpacer), -160f, 0);
            card.GetComponent<DragDrop>().SetInit(i, this);

            cards[i] = card;
            effects[i] = statusParent.GetChild(i).GetComponent<StatusUI>();
        }
    }

    public void SetupUI(Player player, bool isInteractable)
    {
        this.player = player;
        player.OnPlayerChanged += UpdateUI;

        nameText.text = player.playerName;
        healthText.text = $"{player.currHealth}/{player.fullHealth}HP";
        energyText.text = player.energy.ToString();

        if (cards[0] == null)
        {
            InitUI();
        }

        for (int i = 0; i < 5; i++)
        {
            if (i < player.cardHand.Count)
            {
                cards[i].SetupCard(player.cardHand[i], player);
                cards[i].ShowCard(true);
            }
            else
            {
                cards[i].ShowCard(false);
            }

            cards[i].FlipCard(!isInteractable);

            if (i < player.effects.Count)
            {
                effects[i].gameObject.SetActive(true);
                effects[i].SetupEffect(player.effects[i]);
            }
            else
            {
                effects[i].gameObject.SetActive(false);
            }
        }

        cardGroup.interactable = isInteractable;
        cardGroup.blocksRaycasts = isInteractable;

        if (isInteractable)
            StartCoroutine("UpdateTimer");
    }

    private void UpdateUI()
    {
        healthText.text = $"{player.currHealth}/{player.fullHealth}HP";
        LeanTween.value(healthBar.gameObject, healthBar.fillAmount, player.currHealth/(float)player.fullHealth, 0.3f).setOnUpdate( (float val) => { healthBar.fillAmount = val; } );

        if (energyText.text != player.energy.ToString())
        {
            energyText.text = player.energy.ToString();
            LeanTween.scale(energyText.gameObject, new Vector3(1.3f, 1.3f, 1.3f), 0.2f).setLoopPingPong(1);
        }

        for (int i = 0; i < 5; i++)
        {
            if (i < player.cardHand.Count)
            {
                cards[i].SetupCard(player.cardHand[i], player);
                cards[i].ShowCard(true);
            }
            else
            {
                cards[i].ShowCard(false);
            }

            if (i < player.effects.Count)
            {
                effects[i].gameObject.SetActive(true);
                effects[i].SetupEffect(player.effects[i]);
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
            StartCoroutine(MoveCard(message, cardSlot));
        }
        else
        {
            StartCoroutine(RemoveCard(message.cardIndex));
        }
    }

    IEnumerator MoveCard(MoveMessage message, CardSlot cardSlot)
    {
        FightManager.singleton.timeToMakeMove = 0.5f;

        cards[message.cardIndex].FlipCard(false);
        cards[message.cardIndex].transform.SetParent(canvas.transform);
        
        LeanTween.move(cards[message.cardIndex].gameObject, new Vector3(cardSlot.transform.position.x + 172.5f, cardSlot.transform.position.y, cardSlot.transform.position.z), 0.5f);
        
        yield return new WaitForSeconds(0.5f);

        cards[message.cardIndex].GetComponent<DragDrop>().ResetDrag();
        cardSlot.SetupCard(player.cardHand[message.cardIndex], true);

        if (message.cardPlayed)
        {
            cardSlot.PlayAnimation(false);
        }

        player.cardHand.RemoveAt(message.cardIndex);

        UpdateUI();

        FightManager.singleton.timeToMakeMove = 0f;
    }

    IEnumerator RemoveCard(int cardIndex)
    {
        FightManager.singleton.timeToMakeMove = 0.2f;

        Vector3 targetPositon = new Vector3(cards[cardIndex].transform.position.x, cards[cardIndex].transform.position.y + 200f, cards[cardIndex].transform.position.z);
        cards[cardIndex].transform.SetParent(canvas.transform);
        LeanTween.move(cards[cardIndex].gameObject, targetPositon, 0.2f);

        yield return new WaitForSeconds(0.2f);

        cards[cardIndex].GetComponent<DragDrop>().ResetDrag();
        player.cardHand.RemoveAt(cardIndex);

        UpdateUI();

        FightManager.singleton.timeToMakeMove = 0f;
    }

    public void MakeInteractable(bool isInteractable)
    {
        if (isInteractable && !cardGroup.interactable)
        {
            StartCoroutine("UpdateTimer");
        }
        else if (!isInteractable)
        {
            StopCoroutine("UpdateTimer");
            LeanTween.cancel(timer.gameObject);
            timer.fillAmount = 1.0f;
        }

        cardGroup.interactable = isInteractable;
        cardGroup.blocksRaycasts = isInteractable;

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].FlipCard(!isInteractable);
        }
    }

    IEnumerator UpdateTimer()
    {
        int time = GlobalManager.turnTime;
        while (time >= 0)
        {
            time--;
            if (timer.fillAmount > time / (float)GlobalManager.turnTime)
            {
                LeanTween.value(timer.gameObject, timer.fillAmount, time / (float)GlobalManager.turnTime, 1f).setOnUpdate((float val) => { timer.fillAmount = val; });
            }
            else
            {
                timer.fillAmount = time / (float)GlobalManager.turnTime;
            }

            yield return new WaitForSeconds(1.0f);
        }

        FightManager.singleton.SendMove(player.playerID);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnPlayerChanged -= UpdateUI;
        }
    }
}
