using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ImpactUI impactFrame;

    [SerializeField] private CardUI cardUI;
    [SerializeField] private CardUI lastCardUI;

    private float[] startX = new float[] {0, 29, 41, 29, 0, -29, -41, -29};
    private float[] startY = new float[] {41, 29, 0, -29, -41, -29, 0, 29};
    private float[] endX = new float[] {0, 152, 215, 152, 0, -152, -215, -152};
    private float[] endY = new float[] {215, 152, 0, -152, -215, -152, 0, 152};

    private void Start()
    {
        FightManager.singleton.OnMoveFailed += ResetCard;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            CardUI eventCardUI = eventData.pointerDrag.GetComponent<CardUI>();
            eventCardUI.HighlightCard(true);
            eventCardUI.UpdateMoveText(true, cardUI.card.hasMove ? cardUI.card.move.cost : 0);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            CardUI eventCardUI = eventData.pointerDrag.GetComponent<CardUI>();
            eventCardUI.HighlightCard(false);
            eventCardUI.UpdateMoveText(false, 1);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && FightManager.singleton.IsAbleToMessage())
        {
            /*int cardIndex = eventData.pointerDrag.GetComponent<DragDrop>().cardIndex;
            CardUI eventCardUI = eventData.pointerDrag.GetComponent<CardUI>();
            
            SetupCard(eventCardUI.card, eventCardUI.transform.eulerAngles.z == 180f);
            FightManager.singleton.SendMove(cardIndex, true);*/
            StartCoroutine(PlayCard(eventData.pointerDrag.GetComponent<CardUI>(), eventData.pointerDrag.GetComponent<DragDrop>().cardIndex));
        }
    }

    IEnumerator PlayCard(CardUI eventCardUI, int cardIndex)
    {
        if (eventCardUI.card.isSpecial)
        {
            impactFrame.gameObject.SetActive(true);
            impactFrame.SetupUI(eventCardUI.card.fighter.name, eventCardUI.transform.eulerAngles.z == 180f);

            yield return new WaitForSeconds(0.8f);

            impactFrame.gameObject.SetActive(false);
        }

        SetupCard(eventCardUI.card, eventCardUI.transform.eulerAngles.z == 180f);
        FightManager.singleton.SendMove(cardIndex, true);
    }

    public void SetupCard(Card card, bool isFlipped)
    {
        if (cardUI.card.hasMove)
        {
            lastCardUI.SetupCard(cardUI.card);
            lastCardUI.transform.eulerAngles = cardUI.transform.eulerAngles;
        }

        cardUI.transform.eulerAngles = new Vector3(0, 0, isFlipped ? 180 : 0);
        cardUI.SetupCard(card);
    }

    public void ResetCard()
    {
        cardUI.transform.eulerAngles = lastCardUI.transform.eulerAngles;
        cardUI.SetupCard(lastCardUI.card);
    }

    public void PlayAnimation(bool switchCards, bool animate)
    {
        if (switchCards || !cardUI.gameObject.activeSelf)
        {
            cardUI.gameObject.SetActive(!cardUI.gameObject.activeSelf);
        }

        if (animate)
        {
            Card card = switchCards ? lastCardUI.card : cardUI.card;

            if (card.hasMove)
            {
                if (card.move.moveType == MoveType.Standard)
                {
                    if (card.move.moveID%10 == 3)
                    {
                        AudioManager.singleton.PlayPositiveSound();
                    }
                    else if (card.move.moveID%10 == 4)
                    {
                        AudioManager.singleton.PlayNegativeSound();
                    }
                    else
                    {
                        AudioManager.singleton.PlayNeutralSound();
                    }
                }
                else
                {
                    AudioManager.singleton.PlayNeutralSound();
                }
            }
            else
            {
                AudioManager.singleton.PlayNeutralSound();
            }

            for (int i = 0; i < 8; i++)
            {
                LeanTween.moveLocal(transform.GetChild(i).gameObject, new Vector3(endX[i], endY[i], 0f), 0.3f).setOnComplete(ResetAnimation);
            }
        }
    }

    private void ResetAnimation()
    {
        for (int i = 0; i < 8; i++)
        {
            LeanTween.moveLocal(transform.GetChild(i).gameObject, new Vector3(startX[i], startY[i], 0f), 0.1f).setDelay(0.3f);
        }
    }

    public void MoveUp(bool move, bool fullMove = true)
    {
        if (fullMove)
        {
            LeanTween.moveLocalY(gameObject, move ? 160f : -160f, 0.3f);
        }
        else
        {
            LeanTween.moveLocalY(gameObject, 0f, 0.3f);
        }
    }

    private void OnDestroy()
    {
        FightManager.singleton.OnMoveFailed -= ResetCard;
    }
}
