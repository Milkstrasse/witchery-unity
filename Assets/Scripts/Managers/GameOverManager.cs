using System;
using Mirror;
using UnityEngine;
using Utp;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject credits;
    [SerializeField] private FightLogUI fightLog;
    [SerializeField] private GameObject statistics;

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

    public void ToggleCredits(bool isFlipped)
    {
        AudioManager.singleton.PlayStandardSound();

        credits.transform.eulerAngles = new Vector3(0f, 0f, isFlipped ? 180f : 0f);
        credits.SetActive(!credits.activeSelf);
    }

    public void ToggleLog(bool isFlipped)
    {
        AudioManager.singleton.PlayStandardSound();

        fightLog.transform.eulerAngles = new Vector3(0f, 0f, isFlipped ? 180f : 0f);
        fightLog.gameObject.SetActive(!fightLog.gameObject.activeSelf);
    }

    public void ToggleStatistics(bool isFlipped)
    {
        AudioManager.singleton.PlayStandardSound();

        statistics.transform.eulerAngles = new Vector3(0f, 0f, isFlipped ? 180f : 0f);
        statistics.SetActive(!statistics.activeSelf);
        gameOver.SetActive(!statistics.activeSelf);
    }

    public void ReturnToSelection()
    {
        AudioManager.singleton.PlayStandardSound();

        GlobalManager.QuitAnyConnection();

        GlobalManager.singleton.LoadScene("SelectionScene");
    }
}