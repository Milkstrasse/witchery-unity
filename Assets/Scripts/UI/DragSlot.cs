using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardUI cardUI;
    private Card lastCard;

    private void Start()
    {
        FightManager.singleton.OnMoveFailed += ResetCard;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<CardUI>().HighlightCard(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<CardUI>().HighlightCard(false);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && FightManager.singleton.IsAbleToMessage())
        {
            int cardIndex = eventData.pointerDrag.GetComponent<DragDrop>().cardIndex;
            CardUI cardUI = eventData.pointerDrag.GetComponent<CardUI>();
            cardUI.HighlightCard(false);
            
            SetupCard(cardUI.card, false);

            FightManager.singleton.SendMove(cardIndex, true);
        }
    }

    public void SetupCard(Card card, bool isFlipped)
    {
        lastCard = cardUI.card;
        transform.eulerAngles = new Vector3(0, 0, isFlipped ? 180 : 0);

        cardUI.SetupCard(card);
        cardUI.FlipCard(false);
    }

    public void ResetCard()
    {
        transform.eulerAngles = new Vector3(0, 0, 180);
        
        if (lastCard.hasMove)
        {
            cardUI.SetupCard(lastCard);
        }
        else
        {
            cardUI.FlipCard(true);
        }
    }

    private void OnDestroy()
    {
        FightManager.singleton.OnMoveFailed -= ResetCard;
    }
}
