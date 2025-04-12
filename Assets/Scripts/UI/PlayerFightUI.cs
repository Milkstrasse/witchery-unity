using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class PlayerFightUI : MonoBehaviour
{
    public PlayerObject player;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private LocalizeStringEvent healthText;
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI blanksText;
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

    private int roundsPlayed;
    private CPULogic logic;

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
        float cardSpacer = (Screen.width/(canvas.scaleFactor/GlobalData.uiScale) - 5 * 235 - 40)/4 * -1;
        for (int i = 0; i < 5; i++)
        {
            CardUI card = cardParent.transform.GetChild(i).GetComponent<CardUI>();
            card.transform.localPosition = new Vector3(i * (235 - cardSpacer), -160f, 0);
            card.GetComponent<DragDrop>().SetInit();

            cards[i] = card;
            effects[i] = statusParent.GetChild(i).GetComponent<StatusUI>();
        }
    }

    public void SetupUI(int index, bool isInteractable, bool canBePlayable)
    {
        this.player = FightManager.singleton.players[index];
        player.OnPlayerChanged += UpdateUI;

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalData.fighters[player.fighterIDs[0].fighterID].name + "-" + GlobalData.fighters[player.fighterIDs[0].fighterID].outfits[player.fighterIDs[0].outfit].name);

        nameText.text = GlobalData.fighters[player.fighterIDs[0].fighterID].name;
        (healthText.StringReference["currHealth"] as IntVariable).Value = player.currHealth;
        (healthText.StringReference["fullHealth"] as IntVariable).Value = player.fullHealth;
        energyText.text = player.energy.ToString();
        blanksText.text = player.blanks.ToString();

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 120f);

        if (cards[0] == null)
        {
            InitUI();
        }

        logic = new CPULogic(index, 1 - index);

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

            cards[i].FlipCard(!isInteractable || !canBePlayable, 0.2f + i * 0.02f);

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
            StartCoroutine(UpdateTimer());
        }
        else if (GlobalManager.singleton.mode == GameMode.Training && isInteractable && !canBePlayable)
        {
            StartCoroutine(MakeCPUMove());
        }

        LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, isInteractable ? 450f :  120f), 0.3f);
    }

    private void UpdateUI()
    {
        LeanTween.value(healthText.gameObject, (healthText.StringReference["currHealth"] as IntVariable).Value * 1f, player.currHealth, 0.5f).setOnUpdate((float val) => { (healthText.StringReference["currHealth"] as IntVariable).Value = Mathf.RoundToInt(val); });
        LeanTween.value(healthBar.gameObject, healthBar.fillAmount, player.currHealth/(float)player.fullHealth, 0.5f).setOnUpdate((float val) => { healthBar.fillAmount = val; });

        if (energyText.text != player.energy.ToString())
        {
            energyText.text = player.energy.ToString();
            energyText.transform.localScale = Vector3.one;
            LeanTween.scale(energyText.gameObject, new Vector3(1.3f, 1.3f, 1.3f), 0.2f).setLoopPingPong(1);
        }

        if (blanksText.text != player.blanks.ToString())
        {
            blanksText.text = player.blanks.ToString();
            blanksText.transform.localScale = Vector3.one;
            LeanTween.scale(blanksText.gameObject, new Vector3(1.3f, 1.3f, 1.3f), 0.2f).setLoopPingPong(1);
        }

        for (int i = 0; i < 5; i++)
        {
            if (i < player.cardHand.Count)
            {
                cards[i].SetupCard(player.cardHand[i], player);
                cards[i].ShowCard(true);

                if (GlobalData.highlightPlayable)
                {
                    if (!cardSlot.cardWasPlayed && player.cardHand[i].hasMove && player.cardHand[i].move.IsResponseTo(cardSlot.cardUI.card.move, player.energy))
                    {
                        cards[i].FocusCard(true);
                    }
                    else
                    {
                        cards[i].FocusCard(false);
                    }
                }
            }
            else
            {
                cards[i].ShowCard(false);
            }

            if (i < player.effects.Count)
            {
                if (player.effects[i].isNew || player.effects[i].multiplier > 0)
                {
                    effects[i].gameObject.SetActive(true);
                    effects[i].SetupEffect(player.effects[i]);
                }
            }
            else
            {
                if (!LeanTween.isTweening(effects[i].gameObject))
                {
                    effects[i].gameObject.SetActive(false);
                }
            }
        }

        if (player.playerID > 0 && player.roundsPlayed > roundsPlayed)
        {
            GlobalManager.singleton.fightLog.EndRound(player.roundsPlayed);
            roundsPlayed = player.roundsPlayed;
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
        Card card = player.cardHand[message.cardIndex];
        CardUI cardUI = cards[message.cardIndex];

        if (card.isSpecial && GlobalData.animateImpact)
        {
            FightManager.singleton.timeToMakeMove = 1.7f;
        }
        else
        {
            FightManager.singleton.timeToMakeMove = 0.9f;
        }

        yield return new WaitForSeconds(0.3f);

        cardUI.FlipCard(false, 0f);
        cardUI.transform.SetParent(canvas.transform);
        
        //235/2 = 117.5
        LeanTween.move(cardUI.gameObject, new Vector3(cardSlot.transform.position.x + 117.5f * canvas.transform.localScale.x, cardSlot.transform.position.y, cardSlot.transform.position.z), 0.5f);
        
        yield return new WaitForSeconds(0.6f);

        if (card.isSpecial && GlobalData.animateImpact)
        {
            cardSlot.impactFrame.transform.SetAsLastSibling();
            cardSlot.impactFrame.ToggleVisibility(true);
            cardSlot.impactFrame.SetupUI(card.move.target, card.fighter.name, card.fighter.outfits[card.outfit].name, true);

            yield return new WaitForSecondsRealtime(0.8f);

            cardSlot.impactFrame.ToggleVisibility(false);
        }

        cardUI.GetComponent<DragDrop>().ResetDrag();
        cardSlot.SetupCard(card, true);

        cardSlot.PlayAnimation(false, message.cardPlayed, true);

        cardUI.FlipCard(true, 0f);
        player.cardHand.RemoveAt(message.cardIndex);

        UpdateUI();

        FightManager.singleton.timeToMakeMove = 0f;
    }

    IEnumerator RemoveCard(int cardIndex)
    {
        FightManager.singleton.timeToMakeMove = 0.25f;

        cards[cardIndex].transform.SetParent(canvas.transform);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(cards[cardIndex].transform.position);
        Vector3 targetPositon = new Vector3(screenPos.x, screenPos.y + 350f, screenPos.z);
        targetPositon = Camera.main.ScreenToWorldPoint(targetPositon);
        
        LeanTween.move(cards[cardIndex].gameObject, targetPositon, 0.25f);

        yield return new WaitForSeconds(0.25f);

        cards[cardIndex].GetComponent<DragDrop>().ResetDrag();
        player.cardHand.RemoveAt(cardIndex);
        
        AudioManager.singleton.PlayStandardSound();

        UpdateUI();

        FightManager.singleton.timeToMakeMove = 0f;
    }

    public void MakeInteractable(bool isInteractable, bool canBePlayable)
    {
        if (isInteractable && canBePlayable && !cardGroup.interactable)
        {
            StartCoroutine("UpdateTimer");
        }
        else if (isInteractable && !canBePlayable && GlobalManager.singleton.mode == GameMode.Training)
        {
            StartCoroutine(MakeCPUMove());
        }
        else if (!isInteractable)
        {
            StopAllCoroutines();
            LeanTween.cancel(timer.gameObject);
            timer.fillAmount = 1.0f;
        }

        LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, isInteractable ? 450f : 120f), 0.3f);

        cardGroup.interactable = isInteractable && canBePlayable;
        cardGroup.blocksRaycasts = isInteractable && canBePlayable;

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].FlipCard(!isInteractable || !canBePlayable, 0.2f + i * 0.02f);
        }
    }

    IEnumerator UpdateTimer()
    {
        int time = GlobalData.turnTime;
        while (time >= 0)
        {
            time--;
            if (timer.fillAmount > time/(float)GlobalData.turnTime)
            {
                LeanTween.value(timer.gameObject, timer.fillAmount, time/(float)GlobalData.turnTime, 1f).setOnUpdate((float val) => { timer.fillAmount = val; });
            }
            else
            {
                timer.fillAmount = time/(float)GlobalData.turnTime;
            }

            yield return new WaitForSeconds(1.0f);
        }

        FightManager.singleton.SendMove(player.playerID);
    }

    IEnumerator MakeCPUMove()
    {
        yield return new WaitForSeconds(0.4f);

        MoveMessage message = logic.GetMove(player, FightManager.singleton.GetLogic());
        MakeMove(message);

        yield return new WaitForSeconds(message.playCard ? 0.9f : 0.25f);

        if (message.playCard && player.cardHand[message.cardIndex].isSpecial && GlobalData.animateImpact)
        {
            yield return new WaitForSeconds(0.8f);
        }

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
