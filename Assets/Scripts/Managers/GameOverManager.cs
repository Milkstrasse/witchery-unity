using System;
using Mirror;
using UnityEngine;
using Utp;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject credits;
    [SerializeField] private FightLogUI fightLog;
    [SerializeField] private GameObject statistics;
    private Canvas canvas;

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

        canvas = GetComponent<Canvas>();
    }

    public void Rematch()
    {
        if (NetworkClient.activeHost && NetworkServer.connections.Count == GlobalManager.singleton.maxPlayers)
        {
            AudioManager.singleton.PlayPositiveSound();
            NetworkClient.Send(new TurnMessage());
        }
        else if (!NetworkClient.activeHost && NetworkClient.isConnected)
        {
            AudioManager.singleton.PlayPositiveSound();
            NetworkClient.Send(new TurnMessage());
        }
        else
        {
            AudioManager.singleton.PlayNegativeSound();
            GlobalManager.QuitAnyConnection();
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

    public void ToggleLog()
    {
        AudioManager.singleton.PlayStandardSound();

        fightLog.gameObject.SetActive(!fightLog.gameObject.activeSelf);
    }

    public void ReturnToSelection()
    {
        AudioManager.singleton.PlayStandardSound();

        GlobalManager.QuitAnyConnection();

        GlobalManager.singleton.LoadScene("SelectionScene");
    }

    public void ToggleStatistics()
    {
        AudioManager.singleton.PlayStandardSound();

        statistics.SetActive(!statistics.activeSelf);
        canvas.enabled = !statistics.activeSelf;
    }
}
