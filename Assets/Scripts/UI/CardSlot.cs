using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ImpactUI impactFrame;

    public CardUI cardUI;
    [SerializeField] private CardUI lastCardUI;

    private readonly float[] startX = new float[] {0, 29, 41, 29, 0, -29, -41, -29};
    private readonly float[] startY = new float[] {41, 29, 0, -29, -41, -29, 0, 29};
    private readonly float[] endX = new float[] {0, 152, 215, 152, 0, -152, -215, -152};
    private readonly float[] endY = new float[] {215, 152, 0, -152, -215, -152, 0, 152};

    public bool cardWasPlayed;

    private void Start()
    {
        FightManager.singleton.OnMoveFailed += ResetCard;
        cardWasPlayed = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<Drag>() == null)
        {
            CardUI eventCardUI = eventData.pointerDrag.GetComponent<CardUI>();
            eventCardUI.HighlightCard(true);
            eventCardUI.UpdateMoveText(true, cardUI.card.hasMove ? cardUI.card.move.cost : 0);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<Drag>() == null)
        {
            CardUI eventCardUI = eventData.pointerDrag.GetComponent<CardUI>();
            eventCardUI.HighlightCard(false);
            eventCardUI.UpdateMoveText(false, 1);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<Drag>() == null && FightManager.singleton.IsAbleToMessage())
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
                    else if (cardUI.card.hasMove && !eventCardUI.card.move.IsResponseTo(cardUI.card.move, eventCardUI.player.energy))
                    {
                        return;
                    }
                }
            }

            StartCoroutine(PlayCard(eventCardUI, cardIndex));

            //SetupCard(eventCardUI.card, eventCardUI.transform.eulerAngles.z == 180f);
            //FightManager.singleton.SendMove(cardIndex, true);
        }
    }

    IEnumerator PlayCard(CardUI eventCardUI, int cardIndex)
    {
        if (eventCardUI.card.isSpecial && GlobalData.animateImpact)
        {
            eventCardUI.player.cardHand.RemoveAt(cardIndex);
            eventCardUI.player.OnPlayerChanged?.Invoke();

            impactFrame.transform.SetAsLastSibling();
            impactFrame.gameObject.SetActive(true);
            impactFrame.SetupUI(eventCardUI.card.move.target, eventCardUI.card.fighter.name, eventCardUI.card.fighter.outfits[eventCardUI.card.outfit].name, eventCardUI.transform.eulerAngles.z == 180f);

            yield return new WaitForSecondsRealtime(0.8f);

            impactFrame.gameObject.SetActive(false);
        }

        SetupCard(eventCardUI.card, eventCardUI.transform.eulerAngles.z == 180f);
        FightManager.singleton.SendMove(cardIndex, true, !eventCardUI.card.isSpecial || !GlobalData.animateImpact);
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

    public void PlayAnimation(bool switchCards, bool animate, bool cardPlayed)
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
                    if (card.move.effect.multiplier > 0)
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

                        if (card.move.moveID == 14) //give effect
                        {
                            AudioManager.singleton.PlayNegativeSound();
                        }
                        else if (card.move.moveID == 15) //gain effect
                        {
                            AudioManager.singleton.PlayPositiveSound();
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
        else if (cardPlayed)
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
