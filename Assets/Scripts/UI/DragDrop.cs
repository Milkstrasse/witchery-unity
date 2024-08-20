using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    private Canvas canvas;
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 initPosition;
    private Transform initParent;
    private CardUI cardUI;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        cardUI = transform.GetComponent<CardUI>();
    }

    public void SetInit()
    {
        initParent = transform.parent;
        initPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.gameObject.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;

        /*if (rectTransform.position.y < 100 || rectTransform.position.y > Screen.height - 100)
        {
            cardUI.HighlightCard(true);
        }
        else
        {
            cardUI.HighlightCard(false);
        }*/
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool toRemove = rectTransform.position.y < 100 || rectTransform.position.y > Screen.height - 100;
        int cardIndex = cardUI.cardIndex;

        ResetDrag(cardIndex);

        if (toRemove || cardUI.isHighlighted)
        {
            gameObject.SetActive(false);

            Player.localPlayer.MakeMove(cardIndex, !toRemove && cardUI.isHighlighted, true);
        }
    }

    public void ResetDrag(int cardIndex)
    {
        canvasGroup.blocksRaycasts = true;

        transform.SetParent(initParent);
        transform.SetSiblingIndex(cardIndex);
        
        rectTransform.anchoredPosition = initPosition;
    }
}