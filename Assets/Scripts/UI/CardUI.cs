using TMPro;
using UnityEngine;
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

    public Card card;

    public void SetupCard(Fighter fighter)
    {
        portrait.sprite = Resources.Load<Sprite>(fighter.name + "-standard-neutral");
        background.color = neutral;

        icon.text = fighter.fighterID.ToString();
        infoText.text = fighter.name;
    }

    public void SetupCard(Card card)
    {
        this.card = card;

        portrait.sprite = Resources.Load<Sprite>(card.fighter.name + "-standard-neutral");
        background.color = neutral;

        icon.text = card.move.cost.ToString();
        infoText.text = card.fighter.name;
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
}