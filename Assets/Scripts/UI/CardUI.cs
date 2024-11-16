using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    private GameObject[] cardSides;

    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI icon;
    [SerializeField] private RawImage frontBackground;
    [SerializeField] private RawImage gradient;
    [SerializeField] private RawImage backBackground;
    [SerializeField] private TextMeshProUGUI infoText;
    public TextMeshProUGUI animatedIcon;

    [SerializeField] private Material neutralFront;
    [SerializeField] private Material highlighted;
    [SerializeField] private Material selected;
    [SerializeField] private Material neutralBack;
    public bool isSelected;
    public bool isHighlighted;

    private LocalizeStringEvent stringEvent;

    public Card card;
    public Player player;

    private bool isSubscribed;

    private void Start()
    {
        cardSides = new GameObject[] {transform.GetChild(0).gameObject, transform.GetChild(1).gameObject};
        stringEvent = infoText.GetComponent<LocalizeStringEvent>();
    }

    public void SetupCard(Fighter fighter)
    {
        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name + "-standard");

        icon.text = $"<style=IconShadow>{Convert.ToChar((uint) fighter.role)}</style>";

        infoText.text = "";
        stringEvent = infoText.GetComponent<LocalizeStringEvent>();
        (stringEvent.StringReference["name"] as StringVariable).Value = fighter.name;
        (stringEvent.StringReference["role"] as StringVariable).Value = LocalizationSettings.StringDatabase.GetLocalizedString("StringTable", fighter.role.ToString());
        
        stringEvent.StringReference.SetReference("StringTable", "fighterDescr");
    }

    public void SetupCard(Fighter fighter, int outfit, Move move)
    {
        UpdateOutfit(fighter, outfit);

        icon.text = move.cost.ToString();

        stringEvent = infoText.GetComponent<LocalizeStringEvent>();

        (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(move.health) + GetPowerBonus(move.moveID == 15 || move.moveID == 16 || move.moveID == 21), 0);
        (stringEvent.StringReference["energy"] as IntVariable).Value = Math.Max(Math.Abs(move.energy) + GetPowerBonus(move.moveID == 15 || move.moveID == 16 || move.moveID == 21), 0);

        if (move.effect.multiplier != 0)
        {
            uint i = Convert.ToUInt32(move.effect.icon, 16);
            (stringEvent.StringReference["effect"] as StringVariable).Value = Convert.ToChar(i).ToString();
        }
        else
        {
            (stringEvent.StringReference["effect"] as StringVariable).Value = "";
        }

        stringEvent.StringReference.SetReference("StringTable", move.GetDescription());
        stringEvent.RefreshString();
    }

    public void SetupCard(Card card, Player player = null)
    {
        this.card = card;
        this.player = player;

        if (GlobalData.highlightPlayable && !isSubscribed && player != null)
        {
            player.OnPlayerChanged += CheckStatus;
            isSubscribed = true;
            CheckStatus();
        }

        if (card.hasMove)
        {
            cardSides[0].transform.GetChild(1).gameObject.SetActive(false);

            UpdateOutfit(card.fighter, card.outfit);

            icon.text = card.move.cost.ToString();

            (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health) + GetPowerBonus(card.IsSpecialMove), 0);
            (stringEvent.StringReference["energy"] as IntVariable).Value = Math.Max(Math.Abs(card.move.energy) + GetPowerBonus(card.IsSpecialMove), 0);

            if (card.move.effect.multiplier != 0)
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

    private void CheckStatus()
    {
        GetComponent<Button>().interactable = !card.hasMove || card.move.cost <= player.energy;
    }

    public void HighlightCard(bool isHighlighted)
    {
        this.isHighlighted = isHighlighted;
        frontBackground.material = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        gradient.material = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        backBackground.material = isHighlighted ? highlighted : isSelected ? selected : neutralBack;
    }

    public void SelectCard(bool isSelected)
    {
        this.isSelected = isSelected;
        frontBackground.material = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        gradient.material = isHighlighted ? highlighted : isSelected ? selected : neutralFront;
        backBackground.material = isHighlighted ? highlighted : isSelected ? selected : neutralBack;
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
        if (!card.hasMove)
            return;

        if (card.IsSpecialMove) //special move
        {
            if (card.move.target > 0)
            {
                (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health * cardCost) + GetPowerBonus(!update) - FightManager.singleton.players[1 - player.playerID].GetDamageModifier(), 0);
            }
            else
            {
                (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health * cardCost) + GetPowerBonus(!update), 0);
            }

            (stringEvent.StringReference["energy"] as IntVariable).Value = Math.Max(Math.Abs(card.move.energy * cardCost) + GetPowerBonus(!update), 0);
            stringEvent.StringReference.SetReference("StringTable", card.move.GetDescription(update ? 6 : 0));

            stringEvent.RefreshString();
        }
        else if (update && card.move.target > 0)
        {
            (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health) + GetPowerBonus(card.IsSpecialMove) - FightManager.singleton.players[1 - player.playerID].GetDamageModifier(), 0);
        }
        else
        {
            (stringEvent.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health) + GetPowerBonus(card.IsSpecialMove), 0);
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

    public void UpdateOutfit(Fighter fighter, int outfit)
    {
        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name + "-" + fighter.outfits[outfit].name);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnPlayerChanged -= CheckStatus;
        }
    }
}