using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFightUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform cardParent;
    [SerializeField]
    private Transform statusParent;

    [SerializeField]
    private TextMeshProUGUI teamName;
    [SerializeField]
    private TextMeshProUGUI energyAmount;
    [SerializeField]
    private TextMeshProUGUI health;
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Slider timeSlider;

    private Player player;
    private CardUI[] cards;
    private StatusUI[] effects;
    //public Vector3[] cardPositions;
    //public float cardSpacer;
    private bool canPlay;

    private void Start()
    {
        if (transform.position.y < 100)
        {
            teamName.text = GlobalManager.teamName;
        }
    }

    public void SetupUI(Player player, Canvas canvas)
    {
        this.player = player;

        teamName.text = player.teamName;
        energyAmount.text = player.energy.ToString() + " MP";
        health.text = player.currHealth.ToString() + "/" + player.fullHealth.ToString() + " HP";

        GetComponent<CanvasGroup>().blocksRaycasts = false;
        cards = new CardUI[5];
        effects = new StatusUI[5];
        //cardPositions = new Vector3[5];

        float cardSpacer = (Screen.width/canvas.scaleFactor - 5 * 230 - 40)/4 * -1;

        for (int i = 0; i < 5; i++)
        {
            CardUI card = cardParent.transform.GetChild(i).GetComponent<CardUI>();
            card.transform.localPosition = new Vector3(115 + i * (230 - cardSpacer), -160f, 0);
            //cardPositions[i] = card.transform.localPosition;

            //card.GetComponent<DragDrop>().SetClone();
            //dragDrop.playerFightUI = this;

            if (i < player.cardHand.Count)
            {
                Fighter fighter = GlobalManager.singleton.fighters[player.cardHand[i].fighterID];
                card.SetupCard(fighter, player.cardHand[i]);
            }
            else
            {
                card.gameObject.SetActive(false);
            }

            cards[i] = card;
            effects[i] = statusParent.GetChild(i).GetComponent<StatusUI>();
        }

        cardParent.sizeDelta = new Vector2(Screen.width/canvas.scaleFactor - 40, cardParent.sizeDelta.y);
    }

    public void UpdateUI()
    {
        Debug.Log("UPDATING");

        energyAmount.text = player.energy.ToString() + " MP";
        health.text = player.currHealth.ToString() + "/" + player.fullHealth.ToString() + " HP";

        LeanTween.value(healthSlider.gameObject, healthSlider.value, player.currHealth/(float)player.fullHealth, 0.3f).setOnUpdate( (float val) => { healthSlider.value = val; } );

        for (int i = 0; i < 5; i++)
        {
            if (i < player.cardHand.Count)
            {
                Fighter fighter = GlobalManager.singleton.fighters[player.cardHand[i].fighterID];
                cards[i].SetupCard(fighter, player.cardHand[i]);

                cards[i].gameObject.SetActive(true);
            }
            else
            {
                cards[i].gameObject.SetActive(false);
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

    public void StartTurn(bool canPlay, bool isOwned)
    {
        Debug.Log("STARTED TURN");

        this.canPlay = canPlay;

        GetComponent<CanvasGroup>().blocksRaycasts = canPlay && isOwned;
        energyAmount.text = player.energy.ToString() + " MP";
    
        RectTransform rectTransform = GetComponent<RectTransform>();
        LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, canPlay ? 520 : 120), 0.3f).setDelay(0.3f);

        if (canPlay)
        {
            StartCoroutine(StartTimer(isOwned));
        }
    }

    IEnumerator StartTimer(bool isOwned)
    {
        int time = GlobalManager.playTime;

        while (time >= 0 && canPlay)
        {
            time -= 1;
            float newValue = time/(float)GlobalManager.playTime;

            if (newValue < timeSlider.value)
            {
                LeanTween.value(timeSlider.gameObject, timeSlider.value, time/(float)GlobalManager.playTime, 1f ).setOnUpdate( (float val) => { timeSlider.value = val; } );
            }

            yield return new WaitForSeconds(1);
        }

        timeSlider.value = 1;

        if (isOwned && time <= 0)
            GiveUp();
    }

    public void MakeMove(int cardIndex, Vector3 position)
    {
        Debug.Log("MAKING A MOVE");

        cards[cardIndex].transform.SetParent(transform.parent.parent);
        LeanTween.move(cards[cardIndex].gameObject, position, 0.3f).setOnComplete(ResetCard, cardIndex);
    }

    private void ResetCard(object param)
    {
        int cardIndex = (int)param;
        cards[cardIndex].GetComponent<DragDrop>().ResetDrag(cardIndex);
        cards[cardIndex].gameObject.SetActive(false);
    }

    public void GiveUp()
    {
        player.GiveUp();
    }
}