using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class OverviewUI : MonoBehaviour
{
    [SerializeField] private RectTransform fighterRect;

    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI motto;
    [SerializeField] private LocalizeStringEvent outfit;

    [SerializeField] private Transform fighterParent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform placeholder;
    private CardUI[] fighterCards;
    private CardUI[] moveCards;

    private int currCard;
    private bool showingCards;
    private int currOutfit;

    [SerializeField] private Image toggleBackground;
    [SerializeField] private Material neutral;
    [SerializeField] private Material highlighted;

    private void Start()
    {
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

        placeholder.SetAsLastSibling();

        moveCards = fighterRect.transform.GetComponentsInChildren<CardUI>();
        outfit.StringReference.SetReference("StringTable", GlobalData.fighters[currCard].outfits[currOutfit].name);

        SetupFighter();
    }

    private void SetupFighter()
    {
        fighterCards[currCard].HighlightCard(true);

        Fighter fighter = GlobalData.fighters[currCard];

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name + "-standard");
        title.text = fighter.name;

        for (int i = 0; i < moveCards.Length; i++)
        {
            moveCards[i].SetupCard(GlobalData.fighters[currCard], currOutfit, GlobalData.fighters[currCard].moves[i]);
        }
    }

    private void SelectCard(int cardIndex)
    {
        AudioManager.singleton.PlayStandardSound();

        if (currCard == cardIndex)
        {
            return;
        }

        currOutfit = 0;
        outfit.StringReference.SetReference("StringTable", GlobalData.fighters[currCard].outfits[currOutfit].name);

        fighterCards[currCard].UpdateOutfit(GlobalData.fighters[currCard], currOutfit);
        fighterCards[currCard].HighlightCard(false);

        currCard = cardIndex;
        
        SetupFighter();
    }

    public void DecreaseOption()
    {
        AudioManager.singleton.PlayStandardSound();

        if (currOutfit > 0)
        {
            currOutfit--;
        }
        else
        {
            currOutfit = GlobalData.fighters[currCard].outfits.Length - 1;
        }

        SetupFighter();

        Fighter fighter = GlobalData.fighters[currCard];

        fighterCards[currCard].UpdateOutfit(fighter, currOutfit);
        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name + "-" + fighter.outfits[currOutfit].name);
        outfit.StringReference.SetReference("StringTable", fighter.outfits[currOutfit].name);
    }

    public void IncreaseOption()
    {
        AudioManager.singleton.PlayStandardSound();

        if (currOutfit < GlobalData.fighters[currCard].outfits.Length - 1)
        {
            currOutfit++;
        }
        else
        {
            currOutfit = 0;
        }

        SetupFighter();

        Fighter fighter = GlobalData.fighters[currCard];

        fighterCards[currCard].UpdateOutfit(fighter, currOutfit);
        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name + "-" + fighter.outfits[currOutfit].name);
        outfit.StringReference.SetReference("StringTable", fighter.outfits[currOutfit].name);
    }

    /*public void ToggleCards()
    {
        AudioManager.singleton.PlayStandardSound();

        showingCards = !showingCards;

        if (showingCards)
        {
            LeanTween.moveLocalX(fighterRect.gameObject, -fighterRect.sizeDelta.x + 700f, 0.3f);
            toggleBackground.material = highlighted;
        }
        else
        {
            LeanTween.moveLocalX(fighterRect.gameObject, 0f, 0.3f);
            toggleBackground.material = neutral;
        }
    }*/

    public void ReturnToMenu(MenuUI menuUI)
    {
        AudioManager.singleton.PlayStandardSound();

        showingCards = false;

        fighterRect.transform.localPosition = new Vector3(0f, fighterRect.transform.localPosition.y, fighterRect.transform.localPosition.z);
        toggleBackground.material = neutral;

        menuUI.SwitchToMenu(0);
    }
}
