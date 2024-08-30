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
        playersReady++;
        message.name = $"Player {playersReady}";
        players.Add(message);

        if (maxConnections == playersReady)
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

        if (maxConnections == playersReady)
        {
            playersReady = 0;
            NetworkClient.Send(new TurnMessage());
        }
    }

    private void OnClientIsReady(NetworkConnectionToClient conn, TurnMessage message)
    {
        playersReady++;
        
        if (maxConnections == playersReady)
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
            for (int i = 0; i < maxConnections; i++)
            {
                players[i] = FightManager.singleton.SetupPlayer(players[i]);
                NetworkServer.SendToAll(players[i]);
            }
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        playersReady = Math.Max(playersReady - 1, 0);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        playersReady = 0;

        base.OnStopServer();
    }
}
