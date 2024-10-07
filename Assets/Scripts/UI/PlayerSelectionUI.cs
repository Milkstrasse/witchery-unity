using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PlayerSelectionUI : MonoBehaviour
{
    [SerializeField] private SelectionUI selectionUI;

    [SerializeField] private Transform fighterParent;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject cardPrefab;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    [SerializeField] private Image timer;
    [SerializeField] private Button readyButton;
    [SerializeField] private Image portrait;

    [SerializeField] private Button modeButton;
    [SerializeField] private Button actionButton;
    [SerializeField] private Color neutral;
    [SerializeField] private Color highlighted;
    private int currCard;
    private int[] outfits;

    private int[] fighters;
    private int currFilter;
    [SerializeField] private LocalizeStringEvent optionText;
    string[] filters;

    private CardUI[] fighterCards;
    private CardUI[] moveCards;
    private bool isEditing;
    private bool isShowingInfo;

    private void Awake()
    {
        canvasGroup = fighterParent.parent.GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform.eulerAngles.z == 180f)
        {
            portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalManager.singleton.fighters[0].name + "-standard");
        }
        else
        {
            portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalManager.singleton.fighters[1].name + "-standard");
        }

        filters = new string[] {"unfiltered", "attack", "support", "team"};

        int fighterAmount = GlobalManager.singleton.fighters.Length;
        fighters = GlobalManager.singleton.GetFighters(0);
        outfits = new int[fighterAmount];

        fighterCards = new CardUI[fighterAmount];

        for (int i = 0; i < fighterAmount; i++)
        {
            CardUI card = Instantiate(cardPrefab, fighterParent).GetComponent<CardUI>();
            card.SetupCard(GlobalManager.singleton.fighters[i]);

            int iCopy = i;
            card.GetComponent<Button>().onClick.AddListener(() => SelectCard(iCopy, false));

            fighterCards[i] = card;
        }

        currFilter = 0;
        currCard = -1;

        moveCards = new CardUI[0];
    }

    private void SelectCard(int cardIndex, bool editing)
    {
        AudioManager.singleton.PlayStandardSound();

        SelectCard(new SelectedFighter(cardIndex, outfits[cardIndex]), editing);
    }

    public void SelectCard(SelectedFighter fighter, bool editing)
    {
        if (isEditing || editing)
        {
            SelectionResult result = selectionUI.EditTeam(fighter);
            
            fighterCards[fighter.fighterID].UpdateOutfit(GlobalManager.singleton.fighters[fighter.fighterID], fighter.outfit);
            outfits[fighter.fighterID] = fighter.outfit;
            fighterCards[fighter.fighterID].SelectCard(result.wasAdded);

            if (!isEditing)
            {
                modeButton.GetComponentInChildren<TextMeshProUGUI>().text = result.wasAdded ? "\uf068" : "\uf067";
            }

            readyButton.interactable = result.hasTeam;

            if (currFilter == filters.Length - 1)
            {
                UpdateUI(true);
            }
        }
        else
        {
            if (currCard >= 0)
            {
                fighterCards[currCard].HighlightCard(false);
            }

            if (currCard == fighter.fighterID)
            {
                currCard = -1;
                actionButton.interactable = false;
                modeButton.GetComponentInChildren<TextMeshProUGUI>().text = "\uf304";
            }
            else
            {
                currCard = fighter.fighterID;
                fighterCards[currCard].HighlightCard(true);

                actionButton.interactable = true;
                modeButton.GetComponentInChildren<TextMeshProUGUI>().text = fighterCards[currCard].isSelected ? "\uf068" : "\uf067";
            }
        }
    }

    public void ToggleUI(bool isActive, bool collapsable = false)
    {
        canvasGroup.interactable = isActive;

        if (isActive)
        {
            LeanTween.cancel(timer.gameObject);
            timer.fillAmount = 1.0f;
            
            optionText.StringReference.SetReference("StringTable", "ready");
        }
        else
        {
            optionText.StringReference.SetReference("StringTable", "cancel");
        }

        if (GlobalManager.singleton.mode == GameMode.Offline)
        {
            readyButton.interactable = false;
            LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, isActive ? 520f : 130f), 0.3f);
        }
        else if (collapsable)
        {
            LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, isActive ? 520f : 130f), 0.3f);
        }
    }

    public void SetTimer(int time)
    {
        if (timer.fillAmount > time/(float)GlobalSettings.waitTime)
        {
            LeanTween.value(timer.gameObject, timer.fillAmount, time/(float)GlobalSettings.waitTime, 1f ).setOnUpdate( (float val) => { timer.fillAmount = val; } );
        }
        else
        {
            timer.fillAmount = time/(float)GlobalSettings.waitTime;
        }
    }

    private void UpdateUI(bool showTeam)
    {
        optionText.StringReference.SetReference("StringTable", filters[currFilter]);

        int index = 0;
        for (int i = 0; i < fighterCards.Length; i++)
        {
            if (!showTeam && index < fighters.Length && fighters[index] == i)
            {
                fighterCards[i].gameObject.SetActive(true);
                index++;
            }
            else if (showTeam)
            {
                fighterCards[i].gameObject.SetActive(fighterCards[i].isSelected);
            }
            else
            {
                fighterCards[i].gameObject.SetActive(false);
            }
        }
    }

    public void DecreaseOption()
    {
        AudioManager.singleton.PlayStandardSound();

        if (!isShowingInfo)
        {
            int filterLength = filters.Length - 1;
            if (currFilter > 0)
            {
                currFilter--;
            }
            else
            {
                currFilter = filterLength;
            }

            if (currFilter == filterLength)
            {
                UpdateUI(true);
            }
            else
            {
                fighters = GlobalManager.singleton.GetFighters(currFilter);
                UpdateUI(false);
            }

            if (currCard >= 0 && !fighterCards[currCard].gameObject.activeSelf)
            {
                SelectCard(currCard, false);
            }
        }
        else
        {
            Fighter fighter = GlobalManager.singleton.fighters[currCard];

            int arrayLength = GlobalManager.singleton.fighters[currCard].outfits.Length - 1;
            bool outfitFound = false;

            while (!outfitFound)
            {
                if (outfits[currCard] > 0)
                {
                    outfits[currCard]--;
                }
                else
                {
                    outfits[currCard] = arrayLength;
                }

                outfitFound = GlobalSettings.unlocked[currCard, outfits[currCard]];
            }

            optionText.StringReference.SetReference("StringTable", fighter.outfits[outfits[currCard]].name);

            fighterCards[currCard].UpdateOutfit(fighter, outfits[currCard]);
            for (int i = 0; i < moveCards.Length; i++)
            {
                moveCards[i].UpdateOutfit(fighter, outfits[currCard]);
            }

            if (fighterCards[currCard].isSelected)
            {
                selectionUI.EditTeam(currCard, outfits[currCard]);
            }
        }
    }

    public void IncreaseOption()
    {
        AudioManager.singleton.PlayStandardSound();

        if (!isShowingInfo)
        {
            int filterLength = filters.Length - 1;
            if (currFilter < filterLength)
            {
                currFilter++;
            }
            else
            {
                currFilter = 0;
            }

            if (currFilter == filterLength)
            {
                UpdateUI(true);
            }
            else
            {
                fighters = GlobalManager.singleton.GetFighters(currFilter);
                UpdateUI(false);
            }

            if (currCard >= 0 && !fighterCards[currCard].gameObject.activeSelf)
            {
                SelectCard(currCard, false);
            }
        }
        else
        {
            Fighter fighter = GlobalManager.singleton.fighters[currCard];

            int arrayLength = fighter.outfits.Length - 1;
            bool outfitFound = false;

            while (!outfitFound)
            {
                if (outfits[currCard] < arrayLength)
                {
                    outfits[currCard]++;
                }
                else
                {
                    outfits[currCard] = 0;
                }

                outfitFound = GlobalSettings.unlocked[currCard, outfits[currCard]];
            }

            optionText.StringReference.SetReference("StringTable", fighter.outfits[outfits[currCard]].name);

            fighterCards[currCard].UpdateOutfit(fighter, outfits[currCard]);
            for (int i = 0; i < moveCards.Length; i++)
            {
                moveCards[i].UpdateOutfit(fighter, outfits[currCard]);
            }

            if (fighterCards[currCard].isSelected)
            {
                selectionUI.EditTeam(currCard, outfits[currCard]);
            }
        }
    }

    public void ToggleMode()
    {
        AudioManager.singleton.PlayStandardSound();

        if (isEditing)
        {
            isEditing = false;
            modeButton.GetComponent<Image>().color = neutral;
        }
        else if (currCard == -1)
        {
            isEditing = true;
            modeButton.GetComponent<Image>().color = highlighted;
        }
        else
        {
            SelectCard(currCard, true);
        }
    }

    public void SwitchMode()
    {
        RectTransform fighterRect = fighterParent.parent.GetComponent<RectTransform>();
        isShowingInfo = !isShowingInfo;

        if (!isShowingInfo)
        {
            actionButton.GetComponent<Image>().color = neutral;
            optionText.StringReference.SetReference("StringTable", filters[currFilter]);

            LeanTween.moveLocalX(fighterParent.parent.gameObject, -fighterRect.sizeDelta.x * 0.5f, 0.4f);
            LeanTween.moveLocalX(cardParent.parent.gameObject, fighterRect.sizeDelta.x * 0.5f, 0.4f).setOnComplete(DestroyCards);
        }
        else
        {
            actionButton.GetComponent<Image>().color = highlighted;

            Fighter fighter = GlobalManager.singleton.fighters[currCard];
            optionText.StringReference.SetReference("StringTable", fighter.outfits[outfits[currCard]].name);
            
            moveCards = new CardUI[fighter.moves.Length];

            for (int i = 0; i < fighter.moves.Length; i++)
            {
                GameObject card = Instantiate(cardPrefab, cardParent);
                moveCards[i] = card.GetComponent<CardUI>();
                moveCards[i].SetupCard(fighter, outfits[currCard], fighter.moves[i]);
            }

            LeanTween.moveLocalX(fighterParent.parent.gameObject, -fighterRect.sizeDelta.x * 1.5f, 0.4f);
            LeanTween.moveLocalX(cardParent.parent.gameObject, -fighterRect.sizeDelta.x * 0.5f, 0.4f);
        }
    }

    private void DestroyCards()
    {
        foreach (Transform i in cardParent.transform)
        {
            Destroy(i.gameObject);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < GlobalManager.singleton.fighters.Length; i++)
        {
            fighterCards[i].GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}
