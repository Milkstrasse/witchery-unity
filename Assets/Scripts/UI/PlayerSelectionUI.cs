using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PlayerSelectionUI : MonoBehaviour
{
    [SerializeField] private SelectionUI selectionUI;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject cardPrefab;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    [SerializeField] private LocalizeStringEvent stringEvent;
    [SerializeField] private Image timer;
    [SerializeField] private Button readyButton;

    private int[] fighters;
    private int currFilter;
    [SerializeField] private LocalizeStringEvent filterEvent;
    string[] filters;

    private CardUI[] cards;

    private void Start()
    {
        canvasGroup = cardParent.parent.GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        filters = new string[] {"unfiltered", "attack", "support", "team"};

        int fighterAmount = GlobalManager.singleton.fighters.Length;
        fighters = GlobalManager.singleton.GetFighters(0);
        cards = new CardUI[fighterAmount];

        for (int i = 0; i < fighterAmount; i++)
        {
            CardUI card = Instantiate(cardPrefab, cardParent).GetComponent<CardUI>();
            card.SetupCard(GlobalManager.singleton.fighters[i]);

            int iCopy = i;
            card.GetComponent<Button>().onClick.AddListener(() => SelectCard(iCopy));

            cards[i] = card;
        }

        currFilter = 0;
    }

    private void SelectCard(int cardIndex)
    {
        AudioManager.singleton.PlayStandardSound();
        
        (bool, bool) result = selectionUI.EditTeam(cardIndex);
        cards[cardIndex].HighlightCard(result.Item1);

        readyButton.interactable = result.Item2;
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
            readyButton.interactable = isActive;
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
        for (int i = 0; i < cards.Length; i++)
        {
            if (!showTeam && index < fighters.Length && fighters[index] == i)
            {
                cards[i].gameObject.SetActive(true);
                index++;
            }
            else if (showTeam)
            {
                cards[i].gameObject.SetActive(cards[i].isHighlighted);
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }
    }

    public void DecreaseFilter()
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
    }

    public void IncreaseFilter()
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
    }
}
