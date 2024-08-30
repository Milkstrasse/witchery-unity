using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PlayerSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject cardPrefab;
    private CanvasGroup canvasGroup;
    [SerializeField] private LocalizeStringEvent stringEvent;
    [SerializeField] private Slider timer;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button editTeam;
    private TextMeshProUGUI editTeamText;

    private CardUI[] cards;
    private int selectIndex;

    private void Start()
    {
        selectIndex = -1;

        canvasGroup = cardParent.parent.GetComponent<CanvasGroup>();
        editTeamText = editTeam.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

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
        if (selectIndex == cardIndex)
        {
            cards[selectIndex].HighlightCard(false);
            selectIndex = -1;

            editTeam.interactable = false;
            editTeamText.text = "+";

            return;
        }

        if (selectIndex >= 0)
        {
            cards[selectIndex].HighlightCard(false);
        }

        selectIndex = cardIndex;
        cards[selectIndex].HighlightCard(true);

        editTeam.interactable = true;
        editTeamText.text = cards[selectIndex].isSelected ? "-" : "+";
    }

    public void EditTeam(SelectionUI selectionUI)
    {
        bool cardAdded = selectionUI.EditTeam(selectIndex);
        editTeamText.text = cardAdded ? "-" : "+";

        cards[selectIndex].SelectCard(cardAdded);
    }

    public void ToggleUI(bool isActive)
    {
        canvasGroup.interactable = isActive;
        editTeam.interactable = isActive;

        if (isActive)
        {
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

    public void MinimizeUI(bool minimize)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, minimize ? 120f :  520f), 0.3f);
    }

    public void SetTimer(int time)
    {
        LeanTween.value(timer.gameObject, timer.value, time/(float)GlobalManager.waitTime, 1f ).setOnUpdate( (float val) => { timer.value = val; } );
    }
}
