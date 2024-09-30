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
    [SerializeField] private RawImage gradient;
    [SerializeField] private RawImage background;
    [SerializeField] private RawImage backBackground;
    [SerializeField] private TextMeshProUGUI infoText;
    public TextMeshProUGUI animatedIcon;

    [SerializeField] private Color neutralFront;
    [SerializeField] private Color highlighted;
    [SerializeField] private Color selected;
    [SerializeField] private Color neutralBack;
    public bool isSelected;
    public bool isHighlighted;

    private LocalizeStringEvent stringEvent;

    public Card card;
    public Player player;

    private void Start()
    {
        cardSides = new GameObject[] {transform.GetChild(0).gameObject, transform.GetChild(1).gameObject};

        stringEvent = infoText.GetComponent<LocalizeStringEvent>();
    }

    public void SetupCard(Fighter fighter)
    {
        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name);

        uint i = Convert.ToUInt32(fighter.effect.icon, 16);
        icon.text = $"<style=IconShadow>{Convert.ToChar(i)}</style>";

        infoText.text = $"<size=+6>{fighter.name}</size>\n{fighter.role}";
    }

    public void SetupCard(Card card, Player player = null)
    {
        this.card = card;
        this.player = player;

        if (card.hasMove)
        {
            cardSides[0].transform.GetChild(1).gameObject.SetActive(false);

            portrait.sprite = Resources.Load<Sprite>("Sprites/" + card.fighter.name);

            icon.text = card.move.cost.ToString();

            (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health) + GetPowerBonus(card.move.moveID == 15 || card.move.moveID == 16 || card.move.moveID == 21), 0);
            (stringEvent.StringReference["energy"] as IntVariable).Value = Math.Max(Math.Abs(card.move.energy) + GetPowerBonus(card.move.moveID == 15 || card.move.moveID == 16 || card.move.moveID == 21), 0);
            (stringEvent.StringReference["duration"] as IntVariable).Value = card.move.effect.duration;

            if (card.move.effect.duration != 0)
            {
                uint i = Convert.ToUInt32(card.move.effect.icon, 16);
                (stringEvent.StringReference["effect"] as StringVariable).Value = Convert.ToChar(i).ToString();
            }
            else
            {
                (stringEvent.StringReference["effect"] as StringVariable).Value = "";
            }

            stringEvent.StringReference.SetReference("StringTable", card.move.GetDescription());

            stringEvent.RefreshString();
        }
        else
        {
            cardSides[0].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void HighlightCard(bool isHighlighted)
    {
        this.isHighlighted = isHighlighted;
        frontBackground.color = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        gradient.color = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        background.color = isHighlighted ? highlighted : isSelected ? selected : neutralBack;
        backBackground.color = isHighlighted ? highlighted : isSelected ? selected : neutralBack;
    }

    public void SelectCard(bool isSelected)
    {
        this.isSelected = isSelected;
        frontBackground.color = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        gradient.color = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        background.color = isHighlighted ? highlighted : isSelected ? selected : neutralBack;
        backBackground.color = isHighlighted ? highlighted : isSelected ? selected : neutralBack;
    }

    public void FlipCard(bool isFlipped, float delay)
    {
        if (delay == 0f)
        {
            if (isFlipped)
            {
                cardSides[0].transform.eulerAngles = new Vector3(cardSides[0].transform.eulerAngles.x, 90f, cardSides[0].transform.eulerAngles.z);
                cardSides[1].transform.eulerAngles = new Vector3(cardSides[1].transform.eulerAngles.x, 0f, cardSides[1].transform.eulerAngles.z);
            }
            else
            {
                cardSides[0].transform.eulerAngles = new Vector3(cardSides[0].transform.eulerAngles.x, 0f, cardSides[0].transform.eulerAngles.z);
                cardSides[1].transform.eulerAngles = new Vector3(cardSides[1].transform.eulerAngles.x, 90f, cardSides[1].transform.eulerAngles.z);
            }

            return;
        }

        if (!isFlipped)
        {
            StartCoroutine(Flip(0, delay));
        }
        else
        {
            StartCoroutine(Flip(1, delay));
        }
    }

    IEnumerator Flip(int side, float delay)
    {
        if (cardSides[1 - side].transform.eulerAngles.y == 90 || cardSides[side].transform.eulerAngles.y == 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(delay);
        
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
        if (card.hasMove && (card.move.moveID == 15 || card.move.moveID == 16 || card.move.moveID == 21)) //special move
        {
            (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health * cardCost) + GetPowerBonus(!update), 0);
            (stringEvent.StringReference["energy"] as IntVariable).Value = Math.Max(Math.Abs(card.move.energy * cardCost) + GetPowerBonus(!update), 0);
            stringEvent.StringReference.SetReference("StringTable", card.move.GetDescription(update ? 12 : 0));

            stringEvent.RefreshString();
        }
        else if (update && player != null)
        {
            (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health) + GetPowerBonus(card.move.moveID == 15 || card.move.moveID == 16 || card.move.moveID == 21) - FightManager.singleton.players[1 - player.playerID].GetShields(), 0);
        }
        else
        {
            (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health) + GetPowerBonus(card.move.moveID == 15 || card.move.moveID == 16 || card.move.moveID == 21), 0);
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

    public void SetupIcon(StatusEffect effect)
    {
        uint i = Convert.ToUInt32(effect.icon, 16);
        animatedIcon.text = Convert.ToChar(i).ToString();

        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        yield return new WaitForSeconds(0.3f);

        animatedIcon.transform.localScale = Vector3.one;
        LeanTween.scale(animatedIcon.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.2f).setLoopPingPong(1);

        yield return new WaitForSeconds(0.4f);
         animatedIcon.transform.localScale = Vector3.zero;
        animatedIcon.gameObject.SetActive(false);
    }
}