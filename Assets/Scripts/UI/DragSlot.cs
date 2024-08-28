using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardUI cardUI;

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
        if (eventData.pointerDrag != null)
        {
            int cardIndex = eventData.pointerDrag.GetComponent<DragDrop>().cardIndex;
            SetupCard(eventData.pointerDrag.GetComponent<CardUI>().card, false);
            
            FightManager.singleton.SendMove(cardIndex, true);
        }
    }

    public void SetupCard(Card card, bool isFlipped)
    {
        transform.eulerAngles = new Vector3(0, 0, isFlipped ? 180 : 0);

        cardUI.SetupCard(card);
        cardUI.FlipCard(false);
    }
}
