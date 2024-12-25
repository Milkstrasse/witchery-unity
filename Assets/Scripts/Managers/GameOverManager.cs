using System;
using Mirror;
using UnityEngine;
using Utp;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject credits;

    private PlayerObject[] players;
    public event Action<PlayerObject[]> OnSetupComplete;

    private void Start()
    {
        players = new PlayerObject[2];

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players[0] = playerObjects[0].GetComponent<PlayerObject>();
        players[1] = playerObjects[1].GetComponent<PlayerObject>();

        OnSetupComplete?.Invoke(players);

        NetworkServer.ReplaceHandler<TurnMessage>(OnRematch);

        SaveManager.SaveData();
    }

    public void Rematch()
    {
        if (NetworkServer.connections.Count == GlobalManager.singleton.maxPlayers)
        {
            AudioManager.singleton.PlayPositiveSound();
            NetworkClient.Send(new TurnMessage());
        }
        else
        {
            AudioManager.singleton.PlayNegativeSound();
            GlobalManager.singleton.LoadScene("SelectionScene");
        }
    }

    [Server]
    public void OnRematch(NetworkConnectionToClient conn, TurnMessage message)
    {
        GameObject.Find("NetworkManager").GetComponent<RelayNetworkManager>().ServerChangeScene("SelectionScene");
    }

    public void ToggleCredits(bool enable)
    {
        AudioManager.singleton.PlayStandardSound();

        credits.SetActive(enable);
    }

    public void ReturnToSelection()
    {
        AudioManager.singleton.PlayStandardSound();

        GlobalManager.QuitAnyConnection();

        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }
        
        GlobalManager.singleton.mode = GameMode.Offline;
        GlobalManager.singleton.LoadScene("SelectionScene");
    }
}
