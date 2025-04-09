using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private GameObject[] cardSides;

    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI icon;
    [SerializeField] private RawImage frontBackground;
    [SerializeField] private RawImage gradient;
    [SerializeField] private RawImage backBackground;
    [SerializeField] private LocalizeStringEvent infoText;
    public TextMeshProUGUI animatedIcon;

    [SerializeField] private Material neutralFront;
    [SerializeField] private Material highlighted;
    [SerializeField] private Material selected;
    [SerializeField] private Material focused;
    [SerializeField] private Material neutralBack;

    public bool isHighlighted;
    public bool isSelected;
    public bool isFocused;

    public Card card;
    public PlayerObject player;

    private bool isSubscribed;

    public void SetupCard(Fighter fighter)
    {
        bool isUnlocked = SaveManager.savedData.fighters[fighter.fighterID].IsUnlocked();

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name + "-standard");

        if (isUnlocked)
        {
            icon.text = $"<style=IconShadow>{Convert.ToChar((uint)fighter.role)}</style>";

            (infoText.StringReference["effect"] as StringVariable).Value = fighter.name;
            (infoText.StringReference["health"] as IntVariable).Value = fighter.health;

            infoText.StringReference.SetReference("StringTable", "fighterDescr");
        }
        else
        {
            icon.text = "<style=IconShadow>\uf005</style>";
            if (fighter.unlockMission != null)
            {
                if (fighter.unlockMission.goalValue > 0)
                {
                    (infoText.StringReference["amount"] as IntVariable).Value = (int)SaveManager.savedData.GetType().GetField(fighter.unlockMission.checkVariable).GetValue(SaveManager.savedData);
                }

                (infoText.StringReference["health"] as IntVariable).Value = fighter.unlockMission.goalValue;
                infoText.StringReference.SetReference("StringTable", fighter.unlockMission.descrKey);
            }
        }

        GetComponent<Button>().interactable = isUnlocked;
    }

    public void SetupCard(Fighter fighter, int outfit, Move move)
    {
        UpdateOutfit(fighter, outfit);

        icon.text = move.cost.ToString();

        (infoText.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(move.health) + GetPowerBonus(move.moveType == MoveType.Special), 0);
        (infoText.StringReference["energy"] as IntVariable).Value = Math.Max(Math.Abs(move.energy) + GetPowerBonus(move.moveType == MoveType.Special), 0);
        (infoText.StringReference["amount"] as IntVariable).Value = Math.Max(move.effect.multiplier + GetPowerBonus(move.moveType == MoveType.Special), 0);

        if (move.effect.multiplier != 0)
        {
            uint i = Convert.ToUInt32(move.effect.icon, 16);
            (infoText.StringReference["effect"] as StringVariable).Value = Convert.ToChar(i).ToString();
        }
        else
        {
            (infoText.StringReference["effect"] as StringVariable).Value = "";
        }

        infoText.StringReference.SetReference("StringTable", move.GetDescription(false));
        infoText.RefreshString();
    }

    public void SetupCard(Card card, PlayerObject player = null)
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

            (infoText.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health) + GetPowerBonus(card.move.moveType == MoveType.Special), 0);
            (infoText.StringReference["energy"] as IntVariable).Value = Math.Max(Math.Abs(card.move.energy) + GetPowerBonus(card.move.moveType == MoveType.Special), 0);
            (infoText.StringReference["amount"] as IntVariable).Value = Math.Max(card.move.effect.multiplier + GetPowerBonus(card.move.moveType == MoveType.Special), 0);

            if (card.move.effect.multiplier != 0)
            {
                uint i = Convert.ToUInt32(card.move.effect.icon, 16);
                (infoText.StringReference["effect"] as StringVariable).Value = Convert.ToChar(i).ToString();
            }
            else
            {
                (infoText.StringReference["effect"] as StringVariable).Value = "";
            }

            infoText.StringReference.SetReference("StringTable", card.move.GetDescription(false));
            infoText.RefreshString();
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
        SetMaterial();
    }

    public void SelectCard(bool isSelected)
    {
        this.isSelected = isSelected;
        SetMaterial();
    }

    public void FocusCard(bool isFocused)
    {
        this.isFocused = isFocused;
        SetMaterial();
    }

    private void SetMaterial()
    {
        if (isHighlighted)
        {
            frontBackground.material = highlighted;
            gradient.material = highlighted;
            backBackground.material = highlighted;
        }
        else if (isSelected)
        {
            frontBackground.material = selected;
            gradient.material = selected;
            backBackground.material = selected;
        }
        else if (isFocused)
        {
            frontBackground.material = focused;
            gradient.material = focused;
            backBackground.material = focused;
        }
        else
        {
            frontBackground.material = neutralFront;
            gradient.material = neutralFront;
            backBackground.material = neutralBack;
        }
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
        if (cardSides[1 - side].transform.eulerAngles.y == 90f || cardSides[side].transform.eulerAngles.y == 0f)
        {
            yield break;
        }

        yield return new WaitForSeconds(delay);
        
        LeanTween.rotateY(cardSides[1 - side], 90f, 0.1f);
        yield return new WaitForSeconds(0.1f);
        LeanTween.rotateY(cardSides[side], 0f, 0.1f);
    }

    public void ShowCard(bool showCard)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = showCard ? 1f: 0f;
        canvasGroup.blocksRaycasts = showCard;
    }

    public void UpdateMoveText(bool update, int cardCost)
    {
        if (!card.hasMove)
            return;

        if (card.move.moveType == MoveType.Special) //special move
        {
            if (card.move.target > 0)
            {
                (infoText.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health * cardCost) + GetPowerBonus(!update) - FightManager.singleton.players[1 - player.playerID].GetDamageModifier(!update), 0);
            }
            else
            {
                (infoText.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health * cardCost) + GetPowerBonus(!update), 0);
            }

            (infoText.StringReference["energy"] as IntVariable).Value = Math.Max(Math.Abs(card.move.energy * cardCost) + GetPowerBonus(!update), 0);
            (infoText.StringReference["amount"] as IntVariable).Value = Math.Max(card.move.effect.multiplier * cardCost + GetPowerBonus(!update), 0);
            infoText.StringReference.SetReference("StringTable", card.move.GetDescription(update));

            infoText.RefreshString();
        }
        else if (update && card.move.target > 0)
        {
            (infoText.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health) + GetPowerBonus(card.move.moveType == MoveType.Special) - FightManager.singleton.players[1 - player.playerID].GetDamageModifier(!update), 0);
        }
        else
        {
            (infoText.StringReference["health"] as IntVariable).Value = Math.Max(Math.Abs(card.move.health) + GetPowerBonus(card.move.moveType == MoveType.Special), 0);
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