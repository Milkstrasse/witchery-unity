using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ImpactUI impactFrame;

    [SerializeField] private CardUI cardUI;
    [SerializeField] private CardUI lastCardUI;

    private readonly float[] startX = new float[] {0, 29, 41, 29, 0, -29, -41, -29};
    private readonly float[] startY = new float[] {41, 29, 0, -29, -41, -29, 0, 29};
    private readonly float[] endX = new float[] {0, 152, 215, 152, 0, -152, -215, -152};
    private readonly float[] endY = new float[] {215, 152, 0, -152, -215, -152, 0, 152};

    private bool cardWasPlayed;

    private void Start()
    {
        FightManager.singleton.OnMoveFailed += ResetCard;
        cardWasPlayed = true;
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
            CardUI eventCardUI = eventData.pointerDrag.GetComponent<CardUI>();
            int cardIndex = eventData.pointerDrag.GetComponent<DragDrop>().cardIndex;

            if (eventCardUI.card.hasMove)
            {
                if (eventCardUI.card.move.cost > eventCardUI.player.energy)
                {
                    return;
                }
                else if (eventCardUI.card.move.moveType == MoveType.Response)
                {
                    if (cardWasPlayed)
                    {
                        return;
                    }
                    else if (cardUI.card.hasMove && !eventCardUI.player.IsResponse(cardUI.card.move, eventCardUI.card.move))
                    {
                        return;
                    }
                }
            }

            SetupCard(eventCardUI.card, eventCardUI.transform.eulerAngles.z == 180f);
            FightManager.singleton.SendMove(cardIndex, true);

        }
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
            cardWasPlayed = true;

            if (card.hasMove)
            {
                if (card.move.moveType == MoveType.Standard)
                {
                    if (card.move.effect.duration > 0)
                    {
                        if (cardUI.gameObject.activeSelf)
                        {
                            cardUI.animatedIcon.gameObject.SetActive(true);
                            cardUI.SetupIcon(card.move.effect);
                        }
                        else
                        {
                            lastCardUI.animatedIcon.gameObject.SetActive(true);
                            lastCardUI.SetupIcon(card.move.effect);
                        }

                        if (card.move.moveID % 10 == 3)
                        {
                            AudioManager.singleton.PlayPositiveSound();
                        }
                        else if (card.move.moveID % 10 == 4)
                        {
                            AudioManager.singleton.PlayNegativeSound();
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
        else
        {
            cardWasPlayed = false;
            AudioManager.singleton.PlayStandardSound();
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
