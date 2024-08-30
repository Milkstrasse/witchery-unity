using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public Player[] players;
    public event Action<int> OnSetupComplete;

    private void Start()
    {
        players = new Player[2];

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players[0] = playerObjects[0].GetComponent<Player>();
        players[1] = playerObjects[1].GetComponent<Player>();

        OnSetupComplete?.Invoke(players[0].hasWon ? 0 : 1);
    }

    public void ReturnToMenu()
    {
        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }

        GlobalManager.singleton.LoadScene("MenuScene");
    }
}
