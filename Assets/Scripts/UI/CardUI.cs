using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    private GameObject[] cardSides;

    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI icon;
    [SerializeField] private RawImage frontBackground;
    [SerializeField] private RawImage backBackground;
    [SerializeField] private TextMeshProUGUI infoText;

    [SerializeField] private Color neutralFront;
    [SerializeField] private Color highlighted;
    [SerializeField] private Color selected;
    [SerializeField] private Color neutralBack;
    public bool isSelected;
    public bool isHighlighted;

    private LocalizeStringEvent stringEvent;

    public Card card;
    private Player player;

    private void Start()
    {
        cardSides = new GameObject[] {transform.GetChild(0).gameObject, transform.GetChild(1).gameObject};

        stringEvent = infoText.GetComponent<LocalizeStringEvent>();
    }

    public void SetupCard(Fighter fighter)
    {
        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name);

        icon.text = fighter.fighterID.ToString();
        infoText.text = fighter.name;
    }

    public void SetupCard(Card card, Player player = null)
    {
        this.card = card;
        this.player = player;

        if (card.hasMove)
        {
            portrait.sprite = Resources.Load<Sprite>("Sprites/" + card.fighter.name);

            icon.text = card.move.cost.ToString();

            (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health) + GetPowerBonus(card.move.moveID >= 10 && card.move.moveID <= 12), 0);
            (stringEvent.StringReference["energy"] as IntVariable).Value = Math.Max(Math.Abs(card.move.energy) + GetPowerBonus(card.move.moveID >= 10 && card.move.moveID <= 12), 0);
            (stringEvent.StringReference["effect"] as StringVariable).Value = card.move.effect.name;
            stringEvent.StringReference.SetReference("StringTable", card.move.GetDescription());

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
        frontBackground.color = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        backBackground.color = isHighlighted ? highlighted : isSelected ? selected : neutralBack;
    }

    public void SelectCard(bool isSelected)
    {
        this.isSelected = isSelected;
        frontBackground.color = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        backBackground.color = isHighlighted ? highlighted : isSelected ? selected : neutralBack;
    }

    public void FlipCard(bool isFlipped, bool isAnimated = true)
    {
        if (!isAnimated)
        {
            if (card.hasMove)
            {
                if (cardSides[0].transform.eulerAngles.y == 0)
                {
                    cardSides[0].transform.eulerAngles = new Vector3(0, 90, 0);
                    cardSides[1].transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    cardSides[0].transform.eulerAngles = new Vector3(0, 0, 0);
                    cardSides[1].transform.eulerAngles = new Vector3(0, 90, 0);
                }
            }

            return;
        }

        if (card.hasMove && !isFlipped)
        {
            StartCoroutine(Flip(0));
        }
        else
        {
            StartCoroutine(Flip(1));
        }
    }

    IEnumerator Flip(int side)
    {
        if (cardSides[1 - side].transform.eulerAngles.y == 90 || cardSides[side].transform.eulerAngles.y == 0)
        {
            yield break;
        }
        
        LeanTween.rotateY(cardSides[1 - side], 90, 0.1f);
        yield return new WaitForSeconds(0.1f);
        LeanTween.rotateY(cardSides[side], 0, 0.1f);
    }

    public void ShowCard(bool showCard)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = showCard ? 1f: 0;
        canvasGroup.blocksRaycasts = showCard;
    }

    public void UpdateMoveText(bool update, int cardCost)
    {
        if (card.hasMove && card.move.moveID >= 10 && card.move.moveID <= 12)
        {
            (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health * cardCost) + GetPowerBonus(update ? false : card.move.moveID >= 10 && card.move.moveID <= 12), 0);
            (stringEvent.StringReference["energy"] as IntVariable).Value = Math.Max(Math.Abs(card.move.energy * cardCost) + GetPowerBonus(update ? false : card.move.moveID >= 10 && card.move.moveID <= 12), 0);
            stringEvent.StringReference.SetReference("StringTable", card.move.GetDescription(update ? 10 : 0));

            stringEvent.RefreshString();
        }
    }

    private int GetPowerBonus(bool ignoreCost)
    {
        if (player == null || ignoreCost)
        {
            return 0;
        }
        else
        {
            return player.GetPowerBonus();
        }
    }
}