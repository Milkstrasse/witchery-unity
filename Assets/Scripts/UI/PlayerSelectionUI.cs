using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
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
    [SerializeField] private LocalizeStringEvent readyText;
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI playerName;

    [SerializeField] private Button modeButton;
    [SerializeField] private Button actionButton;
    [SerializeField] private Material neutral;
    [SerializeField] private Material highlighted;
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
            portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalData.fighters[SaveManager.savedData.icon].name + "-standard");
            SetName(LocalizationSettings.StringDatabase.GetLocalizedString("StringTable", "player"));
        }
        else
        {
            portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalData.fighters[SaveManager.savedData.icon].name + "-standard");
            SetName(SaveManager.savedData.name);
        }

        filters = new string[] {"unfiltered", "damage", "control", "recovery", "team"};

        int fighterAmount = GlobalData.fighters.Length;
        fighters = GlobalManager.singleton.GetFighters(0);
        outfits = new int[fighterAmount];

        fighterCards = new CardUI[fighterAmount];

        for (int i = 0; i < fighterAmount; i++)
        {
            CardUI card = Instantiate(cardPrefab, fighterParent).GetComponent<CardUI>();
            card.SetupCard(GlobalData.fighters[i]);

            int iCopy = i;
            Button cardButton = card.GetComponent<Button>();
            cardButton.onClick.AddListener(() => SelectCard(iCopy, false));
            cardButton.interactable = SaveManager.savedData.unlocked[i, 0];

            fighterCards[i] = card;
        }

        currFilter = 0;
        currCard = -1;

        moveCards = new CardUI[0];
    }

    public void SetName(string name)
    {
        playerName.text = name;
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

            portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalData.fighters[result.leader.fighterID].name + "-" + GlobalData.fighters[result.leader.fighterID].outfits[result.leader.outfit].name);

            fighterCards[fighter.fighterID].UpdateOutfit(GlobalData.fighters[fighter.fighterID], fighter.outfit);
            outfits[fighter.fighterID] = fighter.outfit;
            fighterCards[fighter.fighterID].SelectCard(result.wasAdded);

            if (fighterCards[fighter.fighterID].isHighlighted)
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
            
            readyText.StringReference.SetReference("StringTable", "ready");
        }
        else
        {
            readyText.StringReference.SetReference("StringTable", "cancel");
        }

        if (GlobalManager.singleton.mode == GameMode.Offline || GlobalManager.singleton.mode == GameMode.Testing)
        {
            readyButton.interactable = false;
            LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, isActive ? 520f : 120f), 0.3f);
        }
        else if (collapsable)
        {
            LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, isActive ? 520f : 120), 0.3f);
        }
    }

    public void SetTimer(int time)
    {
        if (timer.fillAmount > time/(float)GlobalData.waitTime)
        {
            LeanTween.value(timer.gameObject, timer.fillAmount, time/(float)GlobalData.waitTime, 1f ).setOnUpdate( (float val) => { timer.fillAmount = val; } );
        }
        else
        {
            timer.fillAmount = time/(float)GlobalData.waitTime;
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
            Fighter fighter = GlobalData.fighters[currCard];

            int arrayLength = GlobalData.fighters[currCard].outfits.Length - 1;
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

                outfitFound = SaveManager.savedData.unlocked[currCard, outfits[currCard]];
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
            Fighter fighter = GlobalData.fighters[currCard];

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

                outfitFound = SaveManager.savedData.unlocked[currCard, outfits[currCard]];
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
            modeButton.GetComponent<Image>().material = neutral;
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "\uf128";
            actionButton.interactable = false;
        }
        else if (currCard == -1)
        {
            isEditing = true;
            modeButton.GetComponent<Image>().material = highlighted;
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "\uf074";
            actionButton.interactable = true;
        }
        else
        {
            SelectCard(currCard, true);
        }
    }

    private void SelectRandomCard()
    {
        if (currFilter == filters.Length - 1)
        {
            return;
        }

        int cardAmount = fighterCards.Length;

        int[] indices = Enumerable.Range(0, cardAmount).ToArray();
        int n = cardAmount - 1;
        while (n > 0)
        {
            int j = Random.Range(0, n);
            int tmp = indices[n];
            indices[n] = indices[j];
            indices[j] = tmp;

            n--;
        }

        for (int i = 0; i < cardAmount; i++)
        {
            if (!fighterCards[indices[i]].isSelected)
            {
                if (fighterCards[indices[i]].gameObject.activeSelf && fighterCards[indices[i]].GetComponent<Button>().interactable)
                {
                    SelectionResult result = selectionUI.EditTeam(new SelectedFighter(indices[i], 0));

                    if (result.wasAdded)
                    {
                        portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalData.fighters[result.leader.fighterID].name + "-" + GlobalData.fighters[result.leader.fighterID].outfits[result.leader.outfit].name);

                        AudioManager.singleton.PlayStandardSound();
                    }
                    else
                    {
                        AudioManager.singleton.PlayNegativeSound();
                    }

                    fighterCards[indices[i]].SelectCard(result.wasAdded);

                    readyButton.interactable = result.hasTeam;

                    return;
                }
            }
        }

        AudioManager.singleton.PlayNegativeSound();
    }

    public void SwitchMode()
    {
        if (currCard < 0)
        {
            SelectRandomCard();
            return;
        }

        AudioManager.singleton.PlayStandardSound();
        
        RectTransform fighterRect = fighterParent.parent.GetComponent<RectTransform>();
        isShowingInfo = !isShowingInfo;

        if (!isShowingInfo)
        {
            actionButton.GetComponent<Image>().material = neutral;
            optionText.StringReference.SetReference("StringTable", filters[currFilter]);

            LeanTween.moveLocalX(fighterParent.parent.gameObject, -fighterRect.sizeDelta.x * 0.5f, 0.3f);
            LeanTween.moveLocalX(cardParent.parent.gameObject, fighterRect.sizeDelta.x * 0.5f + 20f, 0.3f).setOnComplete(DestroyCards);
        }
        else
        {
            actionButton.GetComponent<Image>().material = highlighted;

            Fighter fighter = GlobalData.fighters[currCard];
            optionText.StringReference.SetReference("StringTable", fighter.outfits[outfits[currCard]].name);
            
            moveCards = new CardUI[fighter.moves.Length];

            for (int i = 0; i < fighter.moves.Length; i++)
            {
                GameObject card = Instantiate(cardPrefab, cardParent);
                moveCards[i] = card.GetComponent<CardUI>();
                moveCards[i].SetupCard(fighter, outfits[currCard], fighter.moves[i]);
                
                moveCards[i].HighlightCard(i == 0);
            }

            LeanTween.moveLocalX(fighterParent.parent.gameObject, -fighterRect.sizeDelta.x * 1.5f - 20f, 0.3f);
            LeanTween.moveLocalX(cardParent.parent.gameObject, -fighterRect.sizeDelta.x * 0.5f, 0.3f);
        }
    }

    private void DestroyCards()
    {
        foreach (Transform i in cardParent.transform)
        {
            Destroy(i.gameObject);
        }
    }

    public void StopSelection()
    {
        if (isShowingInfo)
        {
            SwitchMode();
        }
        else
        {
            selectionUI.StopSelection();
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < GlobalData.fighters.Length; i++)
        {
            fighterCards[i].GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}
