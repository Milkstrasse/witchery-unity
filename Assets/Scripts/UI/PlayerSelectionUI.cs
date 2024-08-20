using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionUI : MonoBehaviour
{
    public List<int> fighterIDs;
    
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private RectTransform fighterParent;
    [SerializeField]
    private RectTransform playerArea;
    [SerializeField]
    private CanvasGroup interactGroup;
    [SerializeField]
    private Button teamButton;

   private int selectIndex;
    private CardUI[] cards;
    //private float cardSpacer;
    //private float initWidth;

    public bool ready;

    public void SetupUI(Canvas canvas)
    {
        selectIndex = -1;
        ready = false;

        cards = new CardUI[GlobalManager.singleton.fighters.Length];

        //cardSpacer = (Screen.width/canvas.scaleFactor - 5 * 230 - 40)/4 * -1 - 15;

        //initWidth = GlobalManager.singleton.fighters.Length * (230 - cardSpacer - 10) + 110;
        //fighterParent.sizeDelta = new Vector2(initWidth, fighterParent.sizeDelta.y);

        for (int i = 0; i < GlobalManager.singleton.fighters.Length; i++)
        {
            GameObject card = Instantiate(cardPrefab, fighterParent.transform);
            //card.transform.localPosition = new Vector3(-initWidth + 115 + i * (230 - cardSpacer - 10), 0, 0);

            int iCopy = i;
            card.GetComponent<Button>().onClick.AddListener( () => SelectCard(iCopy) );

            cards[i] = card.GetComponent<CardUI>();
            cards[i].SetupCard(GlobalManager.singleton.fighters[i]);
        }
    }

    public void ToggleSelection(bool minimize)
    {
        if (minimize)
        {
            SelectCard(selectIndex);
            interactGroup.interactable = false;

            LeanTween.size(playerArea, new Vector2(playerArea.sizeDelta.x, 120), 0.3f);
        }
        else
        {
            interactGroup.interactable = true;
            LeanTween.size(playerArea, new Vector2(playerArea.sizeDelta.x, 520), 0.3f);
        }
    }

    public void SelectCard(int index)
    {
        //ToggleCardHand();

        if (selectIndex != index)
        {
            /*if (index < GlobalManager.singleton.fighters.Length - 1)
            {
                LeanTween.size(fighterParent, new Vector2(initWidth + cardSpacer, fighterParent.sizeDelta.y), 0.3f).setOnComplete(ToggleCardHand);
            }
            else
            {
                LeanTween.size(fighterParent, new Vector2(initWidth, fighterParent.sizeDelta.y), 0.3f).setOnComplete(ToggleCardHand);
            }*/

            if (selectIndex >= 0)
            {
                /*if (index > selectIndex)
                {
                    if (index == GlobalManager.singleton.fighters.Length - 1)
                    {
                        for (int i = selectIndex + 1; i <= index; i++)
                        {
                            LeanTween.moveLocalX(cards[i].gameObject, cards[i].gameObject.transform.localPosition.x - cardSpacer + 95, 0.3f);
                        }
                    }
                    else
                    {
                        for (int i = selectIndex + 1; i <= index; i++)
                        {
                            LeanTween.moveLocalX(cards[i].gameObject, cards[i].gameObject.transform.localPosition.x - cardSpacer, 0.3f);
                        }
                    }
                }
                else
                {
                    if (selectIndex == GlobalManager.singleton.fighters.Length - 1)
                    {
                        for (int i = index + 1; i <= selectIndex; i++)
                        {
                            LeanTween.moveLocalX(cards[i].gameObject, cards[i].gameObject.transform.localPosition.x + cardSpacer - 95, 0.3f);
                        }
                    }
                    else
                    {
                        for (int i = index + 1; i <= selectIndex; i++)
                        {
                            LeanTween.moveLocalX(cards[i].gameObject, cards[i].gameObject.transform.localPosition.x + cardSpacer, 0.3f);
                        }
                    }
                }*/

                cards[selectIndex].HighlightCard(false);
                selectIndex = index;

                teamButton.interactable = true;
            }
            else
            {   
                selectIndex = index;

                /*for (int i = selectIndex + 1; i < GlobalManager.singleton.fighters.Length; i++)
                {
                    LeanTween.moveLocalX(cards[i].gameObject, cards[i].gameObject.transform.localPosition.x + cardSpacer - 95, 0.3f);
                }*/

                teamButton.interactable = true;
            }

            if (cards[selectIndex].isSelected)
            {
                teamButton.GetComponentInChildren<TextMeshProUGUI>().text = "-";
            }
            else
            {
                teamButton.GetComponentInChildren<TextMeshProUGUI>().text = "+";
            }
        }
        else
        {
            /*LeanTween.size(fighterParent, new Vector2(initWidth, fighterParent.sizeDelta.y), 0.3f).setOnComplete(ToggleCardHand);

            if (selectIndex >= 0)
            {
                for (int i = selectIndex + 1; i < GlobalManager.singleton.fighters.Length; i++)
                {
                    LeanTween.moveLocalX(cards[i].gameObject, cards[i].gameObject.transform.localPosition.x - cardSpacer + 95, 0.3f);
                }
            }*/
            
            selectIndex = -1;

            teamButton.interactable = false;
            teamButton.GetComponentInChildren<TextMeshProUGUI>().text = "+";
        }

        if (index >= 0)
            cards[index].HighlightCard(index == selectIndex);
    }

    private void ToggleCardHand()
    {
        CanvasGroup group = fighterParent.GetComponent<CanvasGroup>();
        group.blocksRaycasts = !group.blocksRaycasts;
    }

    public void EditTeam()
    {
        if (selectIndex < 0)
            return;
        
        playerName.text = "Player " + Random.Range(0, 10);

        GlobalManager.teamName = playerName.text;

        int removeIndex = fighterIDs.IndexOf(selectIndex);

        if (removeIndex >= 0)
        {
            fighterIDs.RemoveAt(removeIndex);
            cards[selectIndex].SelectCard(false);

            teamButton.GetComponentInChildren<TextMeshProUGUI>().text = "+";
        }
        else
        {
            fighterIDs.Add(selectIndex);
            cards[selectIndex].SelectCard(true);

            teamButton.GetComponentInChildren<TextMeshProUGUI>().text = "-";
        }
    }

    public void StartFight()
    {
        LeanTween.size(playerArea, new Vector2(playerArea.sizeDelta.x, 120), 0.3f);
    }
}
