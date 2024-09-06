using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField]
    private GameObject cardBack;
    [SerializeField]
    private Image portrait;
    [SerializeField]
    private TextMeshProUGUI icon;
    [SerializeField]
    private RawImage background;
    [SerializeField]
    private TextMeshProUGUI infoText;

    [SerializeField]
    private Color neutral;
    [SerializeField]
    private Color highlighted;
    [SerializeField]
    private Color selected;
    public bool isSelected;
    public bool isHighlighted;

    private LocalizeStringEvent stringEvent;

    public Card card;

    private void Start()
    {
        stringEvent = infoText.GetComponent<LocalizeStringEvent>();
    }

    public void SetupCard(Fighter fighter)
    {
        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name);
        background.color = neutral;

        icon.text = fighter.fighterID.ToString();
        infoText.text = fighter.name;
    }

    public void SetupCard(Card card)
    {
        this.card = card;

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + card.fighter.name);
        background.color = neutral;

        icon.text = card.move.cost.ToString();
        infoText.text = card.move.name;

        stringEvent.StringReference.SetReference("StringTable", card.move.name + "Descr");
        stringEvent.RefreshString();

        cardBack.SetActive(false);
    }

    public void HighlightCard(bool isHighlighted)
    {
        this.isHighlighted = isHighlighted;
        background.color = isHighlighted ? highlighted : isSelected ? selected : neutral;
    }

    public void SelectCard(bool isSelected)
    {
        this.isSelected = isSelected;
    }

    public void FlipCard(bool isFlipped)
    {
        cardBack.SetActive(isFlipped);
    }

    public void ShowCard(bool showCard)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = showCard ? 1f: 0;
        canvasGroup.blocksRaycasts = showCard;
    }
}