using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Utp;

public class CustomNetwork : RelayNetworkManager
{
    private List<PlayerMessage> players;
    private int playersReady;

    public override void OnStartServer()
    {
        base.OnStartServer();

        playersReady = 0;
        players = new List<PlayerMessage>();

        NetworkServer.RegisterHandler<PlayerMessage>(OnServerReceivePlayer);
        NetworkServer.RegisterHandler<TurnMessage>(OnClientIsReady);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        NetworkClient.RegisterHandler<PlayerMessage>(OnClientReceivePlayer);
    }

    private void OnServerReceivePlayer(NetworkConnectionToClient conn, PlayerMessage message)
    {
        message.name = $"Player {playersReady}";
        players.Add(message);

        playersReady++;

        if (playersReady == 2)
        {
            playersReady = 0;
            ServerChangeScene("FightScene");
        }
    }

    private void OnClientReceivePlayer(PlayerMessage message)
    {
        GameObject playerObject = Instantiate(playerPrefab);
        Player player = playerObject.GetComponent<Player>();
        player.SetupPlayer(message, playersReady);

        playersReady++;

        if (playersReady == 2)
        {
            playersReady = 0;
            NetworkClient.Send(new TurnMessage());
        }
    }

    private void OnClientIsReady(NetworkConnectionToClient conn, TurnMessage message)
    {
        playersReady++;
        
        if (playersReady == maxConnections)
        {
            playersReady = 0;
            NetworkServer.SendToAll(new TurnMessage(UnityEngine.Random.Range(0, 2), new PlayerData[0]));
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == "FightScene")
        {
            for (int i = 0; i < 2; i++)
            {
                players[i] = FightManager.singleton.SetupPlayer(players[i]);
                NetworkServer.SendToAll(players[i]);
            }

            networkSceneName = ""; //reset scene so new server can start correctly
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        if (GlobalManager.singleton.GetCurrentScene() == "FightScene")
        {
            StopServer();
            GlobalManager.singleton.LoadScene("MenuScene");
        } else
        {
            playersReady = Math.Max(playersReady - 1, 0);
        }
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        if (GlobalManager.singleton.GetCurrentScene() == "FightScene")
        {
            GlobalManager.singleton.LoadScene("MenuScene");
        }
    }
}
