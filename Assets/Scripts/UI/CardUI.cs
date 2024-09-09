using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private GameObject cardBack;
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI icon;
    [SerializeField] private RawImage background;
    private RawImage cardBackground;
    [SerializeField] private TextMeshProUGUI infoText;

    [SerializeField] private Color neutralFront;
    [SerializeField] private Color highlighted;
    [SerializeField] private Color selected;
    [SerializeField] private Color neutralBack;
    public bool isSelected;
    public bool isHighlighted;

    private LocalizeStringEvent stringEvent;

    public Card card;

    private void Start()
    {
        stringEvent = infoText.GetComponent<LocalizeStringEvent>();
        cardBackground = cardBack.transform.GetChild(0).GetComponent<RawImage>();
    }

    public void SetupCard(Fighter fighter)
    {
        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name);
        background.color = neutralFront;

        icon.text = fighter.fighterID.ToString();
        infoText.text = fighter.name;
    }

    public void SetupCard(Card card)
    {
        this.card = card;

        if (card.hasMove)
        {
            portrait.sprite = Resources.Load<Sprite>("Sprites/" + card.fighter.name);

            icon.text = card.move.cost.ToString();
            stringEvent.StringReference.SetReference("StringTable", card.move.name + "Descr");
            stringEvent.RefreshString();
        }
        else
        {
            FlipCard(false);
        }
    }

    public void HighlightCard(bool isHighlighted)
    {
        this.isHighlighted = isHighlighted;
        background.color = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        cardBackground.color = isHighlighted ? highlighted : isSelected ? selected : neutralBack;
    }

    public void SelectCard(bool isSelected)
    {
        this.isSelected = isSelected;
        background.color = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        cardBackground.color = isHighlighted ? highlighted : isSelected ? selected : neutralBack;
    }

    public void FlipCard(bool isFlipped)
    {
        if (card.hasMove && !isFlipped)
        {
            cardBack.SetActive(false);
        }
        else
        {
            isSelected = false;
            isHighlighted = false;

            background.color = neutralFront;
            cardBackground.color = neutralBack;

            cardBack.SetActive(true);
        }
    }

    public void ShowCard(bool showCard)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = showCard ? 1f: 0;
        canvasGroup.blocksRaycasts = showCard;
    }
}