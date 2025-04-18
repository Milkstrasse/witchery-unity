using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup), typeof(CardUI))]
public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public int cardIndex;

    [SerializeField] private Canvas canvas;

    public PlayerFightUI playerFightUI;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 initPosition;
    private Transform initParent;

    private CardUI cardUI;

    public void SetInit()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        cardUI = GetComponent<CardUI>();

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
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        if (eventData.pointerDrag != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(rectTransform.position);
            cardUI.SelectCard(screenPos.y < 100f || screenPos.y > Screen.height - 100f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool toRemove = cardUI.isSelected;
        ResetDrag();

        if (toRemove && FightManager.singleton.IsAbleToMessage())
        {
            AudioManager.singleton.PlayStandardSound();
            FightManager.singleton.SendMove(cardIndex, false);
        }
    }

    public void ResetDrag()
    {
        cardUI.HighlightCard(false);
        cardUI.SelectCard(false);

        transform.SetParent(initParent);
        rectTransform.anchoredPosition = initPosition;
        transform.SetSiblingIndex(cardIndex);

        canvasGroup.blocksRaycasts = true;
    }
}