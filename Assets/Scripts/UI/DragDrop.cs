using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public int cardIndex;

    [SerializeField]
    private Canvas canvas;
    
    public PlayerFightUI playerFightUI;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 initPosition;
    private Transform initParent;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetInit(int cardIndex, PlayerFightUI playerFightUI)
    {
        this.cardIndex = cardIndex;
        this.playerFightUI = playerFightUI;

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

        if (eventData.pointerDrag != null)
        {
            CardUI cardUI = eventData.pointerDrag.GetComponent<CardUI>();
            cardUI.SelectCard(rectTransform.position.y < 250 || rectTransform.position.y > Screen.height - 250);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool toRemove = rectTransform.position.y < 250 || rectTransform.position.y > Screen.height - 250;
        ResetDrag();

        if (toRemove && FightManager.singleton.IsAbleToMessage())
        {
            FightManager.singleton.SendMove(cardIndex, false);
        }
    }

    public void ResetDrag()
    {
        transform.SetParent(initParent);
        rectTransform.anchoredPosition = initPosition;
        transform.SetSiblingIndex(cardIndex);

        canvasGroup.blocksRaycasts = true;
    }
}
