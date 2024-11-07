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
    [SerializeField] private Image portrait;
    [SerializeField] private Button exitButton;

    [SerializeField] private Transform statusParent;
    private StatusUI[] effects;
    [SerializeField] private Transform cardParent;
    private CanvasGroup cardGroup;
    [SerializeField] private Canvas canvas;
    private CardSlot cardSlot;

    private RectTransform rectTransform;

    private CardUI[] cards;

    private void Start()
    {
        cardGroup = cardParent.GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        cardSlot = canvas.transform.GetChild(0).GetChild(0).GetComponent<CardSlot>();

        cards = new CardUI[5];
        effects = new StatusUI[5];

        InitUI();
    }

    private void InitUI()
    {
        if (rectTransform.eulerAngles.z == 180f)
        {
            portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalManager.singleton.fighters[0].name + "-standard");
        }
        else
        {
            portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalManager.singleton.fighters[1].name + "-standard");
        }

        float cardSpacer = (Screen.width/canvas.scaleFactor - 5 * 230 - 40)/4 * -1;

        for (int i = 0; i < 5; i++)
        {
            CardUI card = cardParent.transform.GetChild(i).GetComponent<CardUI>();
            card.transform.localPosition = new Vector3(i * (230 - cardSpacer), -160f, 0);
            card.GetComponent<DragDrop>().SetInit();

            cards[i] = card;
            effects[i] = statusParent.GetChild(i).GetComponent<StatusUI>();
        }
    }

    public void SetupUI(Player player, bool isInteractable, bool canBePlayable)
    {
        this.player = player;
        player.OnPlayerChanged += UpdateUI;

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalManager.singleton.fighters[player.icon].name + "-standard");

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

            cards[i].FlipCard(!isInteractable || !canBePlayable, 0.2f);

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

        exitButton.interactable = canBePlayable;

        cardGroup.interactable = isInteractable && canBePlayable;
        cardGroup.blocksRaycasts = isInteractable && canBePlayable;

        if (isInteractable && canBePlayable)
        {
            StartCoroutine("UpdateTimer");
        }
        else if ((GlobalManager.singleton.mode == GameMode.Training || GlobalManager.singleton.mode == GameMode.Testing) && isInteractable && !canBePlayable)
        {
            StartCoroutine("MakeCPUMove");
        }

        LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, isInteractable ? 450f :  120f), 0.3f);
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

    public void MakeMove(MoveMessage message)
    {
        if (message.playCard)
        {
            StartCoroutine(MoveCard(message));
        }
        else
        {
            StartCoroutine(RemoveCard(message.cardIndex));
        }
    }

    IEnumerator MoveCard(MoveMessage message)
    {
        FightManager.singleton.timeToMakeMove = 0.8f;
        yield return new WaitForSeconds(0.3f);

        cards[message.cardIndex].FlipCard(false, 0f);
        cards[message.cardIndex].transform.SetParent(canvas.transform);
        
        LeanTween.move(cards[message.cardIndex].gameObject, new Vector3(cardSlot.transform.position.x + 172.5f, cardSlot.transform.position.y, cardSlot.transform.position.z), 0.5f);
        
        yield return new WaitForSeconds(0.5f);

        cards[message.cardIndex].GetComponent<DragDrop>().ResetDrag();
        cardSlot.SetupCard(player.cardHand[message.cardIndex], true);

        cardSlot.PlayAnimation(false, message.cardPlayed);

        cards[message.cardIndex].FlipCard(true, 0f);
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
        
        AudioManager.singleton.PlayStandardSound();

        UpdateUI();

        FightManager.singleton.timeToMakeMove = 0f;
    }

    public void MakeInteractable(bool isInteractable, bool canBePlayable, CardSlot cardSlot = null)
    {
        if (isInteractable && canBePlayable && !cardGroup.interactable)
        {
            StartCoroutine("UpdateTimer");
        }
        else if ((GlobalManager.singleton.mode == GameMode.Training || GlobalManager.singleton.mode == GameMode.Testing) && isInteractable && !canBePlayable)
        {
            StartCoroutine("MakeCPUMove");
        }
        else if (!isInteractable)
        {
            StopCoroutine("UpdateTimer");
            LeanTween.cancel(timer.gameObject);
            timer.fillAmount = 1.0f;
        }

        LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, isInteractable ? 450f : 120f), 0.3f);

        cardGroup.interactable = isInteractable && canBePlayable;
        cardGroup.blocksRaycasts = isInteractable && canBePlayable;

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].FlipCard(!isInteractable || !canBePlayable, 0.2f);
        }
    }

    IEnumerator UpdateTimer()
    {
        int time = GlobalSettings.turnTime;
        while (time >= 0)
        {
            time--;
            if (timer.fillAmount > time / (float)GlobalSettings.turnTime)
            {
                LeanTween.value(timer.gameObject, timer.fillAmount, time / (float)GlobalSettings.turnTime, 1f).setOnUpdate((float val) => { timer.fillAmount = val; });
            }
            else
            {
                timer.fillAmount = time / (float)GlobalSettings.turnTime;
            }

            yield return new WaitForSeconds(1.0f);
        }

        FightManager.singleton.SendMove(player.playerID);
    }

    IEnumerator MakeCPUMove()
    {
        yield return new WaitForSeconds(0.4f);
        MoveMessage message = FightManager.singleton.GetMove();
        MakeMove(message);

        yield return new WaitForSeconds(message.playCard ? 0.8f : 0.2f);

        FightManager.singleton.SendMove(message.cardIndex, message.playCard, false);
    }

    public bool IsActive()
    {
        return rectTransform.sizeDelta.y > 200f;
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnPlayerChanged -= UpdateUI;
        }
    }
}
