using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Drag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 initPosition;
    private Transform initParent;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetInit()
    {
        initParent = transform.parent;
        initPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetInit();
        
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.gameObject.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(initParent);
        rectTransform.anchoredPosition = initPosition;

        canvasGroup.blocksRaycasts = true;
    }
}