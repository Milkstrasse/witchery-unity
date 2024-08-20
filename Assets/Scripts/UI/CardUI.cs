using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public int cardIndex;

    [SerializeField]
    private GameObject cardBack;
    [SerializeField]
    private Image portrait;
    [SerializeField]
    private TextMeshProUGUI info;
    [SerializeField]
    private RawImage background;
    [SerializeField]
    private RawImage gradient;
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

    public void SetupCard(Fighter fighter)
    {
        portrait.sprite = Resources.Load<Sprite>(fighter.name + "-standard-neutral");
        background.color = neutral;
        gradient.color = neutral;

        info.text = fighter.fighterID.ToString();
        infoText.text = fighter.name;
    }

    public void SetupCard(Fighter fighter, Card card, bool isFlipped = false)
    {
        portrait.sprite = Resources.Load<Sprite>(fighter.name + "-standard-neutral");
        background.color = neutral;
        gradient.color = neutral;

        info.text = card.cost.ToString();
        infoText.text = fighter.moves[card.moveID].name;

        isHighlighted = false;
        isSelected = false;

        cardBack.SetActive(isFlipped);

        if (TryGetComponent(out DragDrop dragDrop))
        {
            dragDrop.SetInit();
        }
    }

    public void HighlightCard(bool _isHighlighted)
    {
        isHighlighted = _isHighlighted;
        background.color = isHighlighted ? highlighted : isSelected ? selected : neutral;
        gradient.color = background.color;
    }

    public void SelectCard(bool selected)
    {
        isSelected = selected;
    }
}
