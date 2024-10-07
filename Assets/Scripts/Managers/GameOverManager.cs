using System;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public Player[] players;
    public event Action<string> OnSetupComplete;

    private void Start()
    {
        players = new Player[2];

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players[0] = playerObjects[0].GetComponent<Player>();
        players[1] = playerObjects[1].GetComponent<Player>();

        OnSetupComplete?.Invoke(players[0].hasWon ? players[0].playerName : players[1].playerName);
    }

    public void Rematch()
    {
        AudioManager.singleton.PlayStandardSound();
        GlobalManager.singleton.LoadScene("SelectionScene");
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
