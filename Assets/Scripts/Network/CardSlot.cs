using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : NetworkBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private CardUI cardUI;

    private void Awake()
    {
        transform.SetParent(GameObject.Find("Canvas").transform);

        transform.localPosition = Vector2.zero;
        transform.localScale = Vector3.one;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            int cardIndex = eventData.pointerDrag.GetComponent<CardUI>().cardIndex;

            cardUI.SetupCard(GlobalManager.singleton.fighters[Player.localPlayer.cardHand[cardIndex].fighterID], Player.localPlayer.cardHand[cardIndex]);
            cardUI.transform.eulerAngles = Vector3.zero;
        }
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

    [ClientRpc]
    public void RpcUpdateCard(int playerIndex, Card card)
    {
        Player.localPlayer.onPlayerChanged.Invoke();
        
        cardUI.SetupCard(GlobalManager.singleton.fighters[card.fighterID], card);

        if (NetworkClient.activeHost)
        {
            cardUI.transform.eulerAngles = new Vector3(0, 0, playerIndex == 0 ? 0 : 180f);
        }
        else
        {
            cardUI.transform.eulerAngles = new Vector3(0, 0, playerIndex == 0 ? 180f : 0);
        }
    }
}