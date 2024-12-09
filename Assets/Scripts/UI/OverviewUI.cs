using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverviewUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;

    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI motto;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject infoButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private RectTransform fighterParent;
    [SerializeField] private RectTransform cardsParent;
    private RectTransform rectTransform;
    private CardUI[] fighterCards;

    private int currCard;

    private void Start()
    {
        manager.OnFightersUpdated += UpdateFighters;

        int fighterAmount = GlobalData.fighters.Length;
        fighterCards = new CardUI[fighterAmount];

        for (int i = 0; i < fighterAmount; i++)
        {
            CardUI card = Instantiate(cardPrefab, fighterParent).GetComponent<CardUI>();
            card.SetupCard(GlobalData.fighters[i]);

            int iCopy = i;
            card.GetComponent<Button>().onClick.AddListener(() => SelectCard(iCopy));

            fighterCards[i] = card;
        }

        SetupFighter();

        rectTransform = fighterParent.parent as RectTransform;
        fighterParent.sizeDelta = new Vector2(fighterParent.sizeDelta.x, 300f * Mathf.Ceil(GlobalData.fighters.Length/3f) + (Mathf.Ceil(GlobalData.fighters.Length/3f - 1f) * 24f));
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 300f * Mathf.Ceil(GlobalData.fighters.Length/3f) + (Mathf.Ceil(GlobalData.fighters.Length/3f - 1f) * 24f));
    }

    private void UpdateFighters()
    {
        for (int i = 0; i < fighterCards.Length; i++)
        {
            fighterCards[i].SetupCard(GlobalData.fighters[i]);
        }
    }

    private void SetupFighter()
    {
        fighterCards[currCard].HighlightCard(true);

        Fighter fighter = GlobalData.fighters[currCard];

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name + "-standard");
        title.text = fighter.name;
    }

    private void SelectCard(int cardIndex)
    {
        AudioManager.singleton.PlayStandardSound();

        if (currCard == cardIndex)
        {
            return;
        }

        fighterCards[currCard].HighlightCard(false);

        currCard = cardIndex;
        
        SetupFighter();
    }

    public void ToggleInfo(bool showInfo)
    {
        infoButton.SetActive(!showInfo);
        backButton.SetActive(showInfo);

        if (showInfo)
        {
            foreach (Transform card in cardsParent)
            {
                Destroy(card.gameObject);
            }

            int cardAmount = GlobalData.fighters[currCard].moves.Length;
            for (int i = 0; i < cardAmount; i++)
            {
                CardUI card = Instantiate(cardPrefab, cardsParent).GetComponent<CardUI>();
                card.SetupCard(GlobalData.fighters[currCard], 0, GlobalData.fighters[currCard].moves[i]);
            }

            LeanTween.moveLocalX(fighterParent.gameObject, -fighterParent.sizeDelta.x * 1.5f - 24f, 0.3f);
            LeanTween.moveLocalX(cardsParent.gameObject, -fighterParent.sizeDelta.x * 0.5f, 0.3f);

            cardsParent.sizeDelta = new Vector2(cardsParent.sizeDelta.x, 300f * Mathf.Ceil(cardAmount/3f) + (Mathf.Ceil(cardAmount/3f - 1f) * 24f));
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 300f * Mathf.Ceil(cardAmount/3f) + (Mathf.Ceil(cardAmount/3f - 1f) * 24f));
        }
        else
        {
            LeanTween.moveLocalX(cardsParent.gameObject, fighterParent.sizeDelta.x * 0.5f + 24f, 0.3f);
            LeanTween.moveLocalX(fighterParent.gameObject, -fighterParent.sizeDelta.x * 0.5f, 0.3f);

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 300f * Mathf.Ceil(GlobalData.fighters.Length/3f) + (Mathf.Ceil(GlobalData.fighters.Length/3f - 1f) * 24f));
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    private void OnDestroy()
    {
        manager.OnFightersUpdated -= UpdateFighters;
    }
}
