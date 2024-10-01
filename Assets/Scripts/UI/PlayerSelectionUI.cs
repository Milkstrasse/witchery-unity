using System.Linq;
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
    [SerializeField] private LocalizeStringEvent stringEvent;
    [SerializeField] private Image timer;
    [SerializeField] private Button readyButton;

    [SerializeField] private Button editButton;
    [SerializeField] private GameObject fighterOptions;
    [SerializeField] private GameObject cardOptions;
    [SerializeField] private Color neutral;
    [SerializeField] private Color highlighted;
    private int currFighter;
    private int[] outfits;

    private int[] fighters;
    private int currFilter;
    [SerializeField] private LocalizeStringEvent filterEvent;
    string[] filters;

    private CardUI[] fighterCards;
    private CardUI[] moveCards;
    private bool isEditing;

    private void Start()
    {
        canvasGroup = fighterParent.parent.GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

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
            card.GetComponent<Button>().onClick.AddListener(() => SelectCard(iCopy));

            fighterCards[i] = card;
        }

        currFilter = 0;
        currFighter = 0;

        moveCards = new CardUI[0];
    }

    private void SelectCard(int cardIndex)
    {
        AudioManager.singleton.PlayStandardSound();

        if (!isEditing)
        {
            SelectionResult result = selectionUI.EditTeam(new SelectedFighter(cardIndex, outfits[cardIndex]));
            fighterCards[cardIndex].SelectCard(result.wasAdded);

            readyButton.interactable = result.hasTeam;

            if (currFilter == filters.Length - 1)
            {
                UpdateUI(true);
            }
        }
        else
        {
            SwitchMode(cardIndex);
        }
    }

    public void SelectRandomCard()
    {
        AudioManager.singleton.PlayStandardSound();
        
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
                SelectionResult result = selectionUI.EditTeam(new SelectedFighter(indices[i], outfits[indices[i]]));
                fighterCards[indices[i]].SelectCard(result.wasAdded);

                readyButton.interactable = result.hasTeam;

                if (currFilter == filters.Length - 1)
                {
                    UpdateUI(true);
                }

                return;
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
            
            stringEvent.StringReference.SetReference("StringTable", "ready");
        }
        else
        {
            stringEvent.StringReference.SetReference("StringTable", "cancel");
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
        if (timer.fillAmount > time/(float)GlobalManager.waitTime)
        {
            LeanTween.value(timer.gameObject, timer.fillAmount, time/(float)GlobalManager.waitTime, 1f ).setOnUpdate( (float val) => { timer.fillAmount = val; } );
        }
        else
        {
            timer.fillAmount = time/(float)GlobalManager.waitTime;
        }
    }

    private void UpdateUI(bool showTeam)
    {
        filterEvent.StringReference.SetReference("StringTable", filters[currFilter]);

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

    public void DecreaseFilter()
    {
        AudioManager.singleton.PlayStandardSound();

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
    }

    public void IncreaseFilter()
    {
        AudioManager.singleton.PlayStandardSound();

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
    }

    public void ToggleMode()
    {
        AudioManager.singleton.PlayStandardSound();

        if (isEditing)
        {
            SwitchMode(-1);
            isEditing = false;
        }
        else
        {
            isEditing = true;
        }

        editButton.GetComponent<Image>().color = isEditing ? highlighted : neutral;
    }

    public void SwitchMode(int cardIndex)
    {
        RectTransform fighterRect = fighterParent.parent.GetComponent<RectTransform>();

        if (cardIndex < 0)
        {
            fighterCards[currFighter].HighlightCard(false);

            LeanTween.moveLocalX(fighterOptions, -fighterRect.sizeDelta.x * 0.5f, 0.3f);
            LeanTween.moveLocalX(cardOptions, fighterRect.sizeDelta.x * 0.5f, 0.3f);
            LeanTween.moveLocalX(fighterParent.parent.gameObject, -fighterRect.sizeDelta.x * 0.5f, 0.3f);
            LeanTween.moveLocalX(cardParent.parent.gameObject, fighterRect.sizeDelta.x * 0.5f, 0.3f).setOnComplete(DestroyCards);
        }
        else
        {
            currFighter = cardIndex;
            fighterCards[currFighter].HighlightCard(true);

            Fighter fighter = GlobalManager.singleton.fighters[cardIndex];
            moveCards = new CardUI[fighter.moves.Length];

            for (int i = 0; i < fighter.moves.Length; i++)
            {
                GameObject card = Instantiate(cardPrefab, cardParent);
                moveCards[i] = card.GetComponent<CardUI>();
                moveCards[i].SetupCard(fighter, outfits[currFighter], fighter.moves[i]);
            }

            LeanTween.moveLocalX(fighterOptions, -fighterRect.sizeDelta.x * 1.5f, 0.3f);
            LeanTween.moveLocalX(cardOptions, -fighterRect.sizeDelta.x * 0.5f, 0.3f);
            LeanTween.moveLocalX(fighterParent.parent.gameObject, -fighterRect.sizeDelta.x * 1.5f, 0.3f);
            LeanTween.moveLocalX(cardParent.parent.gameObject, -fighterRect.sizeDelta.x * 0.5f, 0.3f);
        }
    }

    public void DecreaseOutfit()
    {
        AudioManager.singleton.PlayStandardSound();

        Fighter fighter = GlobalManager.singleton.fighters[currFighter];

        int arrayLength = GlobalManager.singleton.fighters[currFighter].outfits.Length - 1;
        if (outfits[currFighter] > 0)
        {
            outfits[currFighter]--;
        }
        else
        {
            outfits[currFighter] = arrayLength;
        }

        fighterCards[currFighter].UpdateOutfit(fighter, outfits[currFighter]);
        for (int i = 0; i < moveCards.Length; i++)
        {
            moveCards[i].UpdateOutfit(fighter, outfits[currFighter]);
        }
    }

    public void IncreaseOutfit()
    {
        AudioManager.singleton.PlayStandardSound();

        Fighter fighter = GlobalManager.singleton.fighters[currFighter];

        int arrayLength = fighter.outfits.Length - 1;
        if (outfits[currFighter] < arrayLength)
        {
            outfits[currFighter]++;
        }
        else
        {
            outfits[currFighter] = 0;
        }

        fighterCards[currFighter].UpdateOutfit(fighter, outfits[currFighter]);
        for (int i = 0; i < moveCards.Length; i++)
        {
            moveCards[i].UpdateOutfit(fighter, outfits[currFighter]);
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
