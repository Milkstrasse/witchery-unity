using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PlayerSelectionUI : MonoBehaviour
{
    [SerializeField] private SelectionUI selectionUI;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject cardPrefab;
    private CanvasGroup canvasGroup;
    [SerializeField] private LocalizeStringEvent stringEvent;
    [SerializeField] private Image timer;
    [SerializeField] private Button readyButton;

    private CardUI[] cards;

    private void Start()
    {
        canvasGroup = cardParent.parent.GetComponent<CanvasGroup>();

        int fighterAmount = GlobalManager.singleton.fighters.Length;
        cards = new CardUI[fighterAmount];

        for (int i = 0; i < fighterAmount; i++)
        {
            CardUI card = Instantiate(cardPrefab, cardParent).GetComponent<CardUI>();
            card.SetupCard(GlobalManager.singleton.fighters[i]);

            int iCopy = i;
            card.GetComponent<Button>().onClick.AddListener(() => SelectCard(iCopy));

            cards[i] = card;
        }
    }

    private void SelectCard(int cardIndex)
    {
        (bool, bool) result = selectionUI.EditTeam(cardIndex);
        cards[cardIndex].HighlightCard(result.Item1);

        readyButton.interactable = result.Item2;
    }

    public void ToggleUI(bool isActive)
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

        if (GlobalManager.singleton.maxPlayers < 2)
        {
            readyButton.interactable = isActive;
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
}
