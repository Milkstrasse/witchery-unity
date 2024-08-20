using System.Collections;
using Mirror;
using UnityEngine;

public class FightUI : MonoBehaviour
{
    [SerializeField]
    private FightManager manager;

    [SerializeField]
    private PlayerFightUI playerTop;
    [SerializeField]
    private PlayerFightUI playerBottom;
    private CardSlot slot;

    private int ownedPosition;

    public void Setup(FightManager manager)
    {
        this.manager = manager;

        manager.players[0].onTurnChange += ChangeTurn;
        manager.players[1].onTurnChange += ChangeTurn;
        manager.players[0].onPlayerChanged += ChangeCards;
        manager.players[1].onPlayerChanged += ChangeCards;

        StartCoroutine(SetupPlayers());
    }

    IEnumerator SetupPlayers()
    {
        Canvas canvas = GetComponent<Canvas>();
        while (manager.players[0].teamName == "" || manager.players[1].teamName == "") //wait on server sync
        {
            yield return null;
        }
        
        if (manager.players[0].GetComponent<NetworkIdentity>().isOwned)
        {
            playerTop.SetupUI(manager.players[1], canvas);
            playerBottom.SetupUI(manager.players[0], canvas);

            ownedPosition = 0;
        }
        else if (manager.players[1].GetComponent<NetworkIdentity>().isOwned)
        {
            playerTop.SetupUI(manager.players[0], canvas);
            playerBottom.SetupUI(manager.players[1], canvas);

            ownedPosition = 1;
        }

        slot = transform.GetChild(transform.childCount - 1).GetComponent<CardSlot>();
    }

    private void ChangeTurn(int turn)
    {
        Debug.Log($"Turn changed to {turn}");

        if (ownedPosition == 0)
        {
            playerTop.StartTurn(turn == 1, false);
            playerBottom.StartTurn(turn == 0, true);

            LeanTween.moveLocalY(slot.gameObject, turn == 0 ? 200f : -200f, 0.3f).setDelay(0.3f);
        }
        else
        {
            playerTop.StartTurn(turn == 0, false);
            playerBottom.StartTurn(turn == 1, true);

            LeanTween.moveLocalY(slot.gameObject, turn == 0 ? -200f : 200f, 0.3f).setDelay(0.3f);
        }
    }

    private void ChangeCards()
    {
        Debug.Log("CARDS CHANGED");

        playerTop.UpdateUI();
        playerBottom.UpdateUI();
    }

    public void MakeMove(int playerIndex, int cardIndex)
    {
        if (playerIndex == ownedPosition)
        {
            playerBottom.MakeMove(cardIndex, slot.transform.position);
        }
        else
        {
            playerTop.MakeMove(cardIndex, slot.transform.position);
        }
    }
}
