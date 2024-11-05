using System;
using Mirror;
using UnityEngine;
using Utp;

public class GameOverManager : MonoBehaviour
{
    private Player[] players;
    public event Action<Player[]> OnSetupComplete;

    private void Start()
    {
        players = new Player[2];

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players[0] = playerObjects[0].GetComponent<Player>();
        players[1] = playerObjects[1].GetComponent<Player>();

        OnSetupComplete?.Invoke(players);

        NetworkServer.ReplaceHandler<TurnMessage>(OnRematch);

        if (NetworkClient.activeHost)
        {
            if (players[0].hasWon)
            {
                GlobalSettings.money += 25;
            }
            else
            {
                GlobalSettings.money += 5;
            }
        }
        else
        {
            if (players[1].hasWon)
            {
                GlobalSettings.money += 25;
            }
            else
            {
                GlobalSettings.money += 5;
            }
        }

        SaveManager.SaveData();
    }

    public void Rematch()
    {
        AudioManager.singleton.PlayStandardSound();
        NetworkClient.Send(new TurnMessage());
    }

    [Server]
    public void OnRematch(NetworkConnectionToClient conn, TurnMessage message)
    {
        GameObject.Find("NetworkManager").GetComponent<RelayNetworkManager>().ServerChangeScene("SelectionScene");
    }

    public void ReturnToMenu()
    {
        AudioManager.singleton.PlayStandardSound();

        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }
        
        GlobalManager.singleton.LoadScene("MenuScene");
    }
}
