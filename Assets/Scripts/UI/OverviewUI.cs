using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class OverviewUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;

    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI motto;

    [SerializeField] private GameObject cardPrefab;
    private CardUI[] fighterCards;

    private int currCard;

    private void Start()
    {
        manager.OnFightersUpdated += UpdateFighters;

        int fighterAmount = GlobalData.fighters.Length;

        fighterCards = new CardUI[fighterAmount];

        for (int i = 0; i < fighterAmount; i++)
        {
            CardUI card = Instantiate(cardPrefab, transform).GetComponent<CardUI>();
            card.SetupCard(GlobalData.fighters[i]);

            int iCopy = i;
            card.GetComponent<Button>().onClick.AddListener(() => SelectCard(iCopy));

            fighterCards[i] = card;
        }

        SetupFighter();
        RectTransform rectTransform = transform as RectTransform;
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

    private void OnDestroy()
    {
        manager.OnFightersUpdated -= UpdateFighters;
    }
}
