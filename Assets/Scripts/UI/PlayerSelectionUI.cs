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
    [SerializeField] private Image timer;
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
            editTeamText.text = "\uf067";

            return;
        }

        if (selectIndex >= 0)
        {
            cards[selectIndex].HighlightCard(false);
        }

        selectIndex = cardIndex;
        cards[selectIndex].HighlightCard(true);

        editTeam.interactable = true;
        editTeamText.text = cards[selectIndex].isSelected ? "\uf068" : "\uf067";
    }

    public void EditTeam(SelectionUI selectionUI)
    {
        bool cardAdded = selectionUI.EditTeam(selectIndex);
        editTeamText.text = cardAdded ? "\uf068" : "\uf067";

        cards[selectIndex].SelectCard(cardAdded);
    }

    public void ToggleUI(bool isActive)
    {
        canvasGroup.interactable = isActive;
        editTeam.interactable = isActive;

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
