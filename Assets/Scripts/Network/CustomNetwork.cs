using System;
using Mirror;
using UnityEngine;
using Utp;

public class CustomNetwork : RelayNetworkManager
{
    private PlayerMessage[] players;
    private int playersReady;

    public override void OnStartServer()
    {
        base.OnStartServer();

        playersReady = 0;
        players = new PlayerMessage[2];

        NetworkServer.RegisterHandler<PlayerMessage>(OnServerReceivePlayer);
        NetworkServer.RegisterHandler<TurnMessage>(OnClientIsReady);

        GlobalManager.singleton.StoreRelayCode(relayJoinCode);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        NetworkClient.RegisterHandler<PlayerMessage>(OnClientReceivePlayer);
    }

    private void OnServerReceivePlayer(NetworkConnectionToClient conn, PlayerMessage message)
    {
        if (conn.connectionId != NetworkServer.localConnection.connectionId)
        {
            players[1] = message;
        }
        else if (players[1].fighterIDs != null || players[0].fighterIDs == null)
        {
            players[0] = message;
        }
        else
        {
            players[1] = message;
        }

        playersReady++;

        if (playersReady == 2)
        {
            playersReady = 0;
            NetworkServer.SendToAll(new TurnMessage());

            Invoke("ChangeToFightScene", 0.3f);
        }
    }

    private void ChangeToFightScene()
    {
        for (int i = 0; i < 2; i++)
        {
            players[i].health = GlobalData.fighters[players[i].fighterIDs[0].fighterID].health;
            players[i] = FightManager.singleton.SetupPlayer(players[i]);
            NetworkServer.SendToAll(players[i]);
        }

        //reset scene so new server can start correctly
        networkSceneName = "";
        playersReady = 0;
        players = new PlayerMessage[2];
    }

    private void OnClientReceivePlayer(PlayerMessage message)
    {
        GameObject playerObject = Instantiate(playerPrefab);
        PlayerObject player = playerObject.GetComponent<PlayerObject>();
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

        if (sceneName == "SelectionScene")
        {
            NetworkServer.ReplaceHandler<PlayerMessage>(OnServerReceivePlayer);
            NetworkServer.ReplaceHandler<TurnMessage>(OnClientIsReady);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        if (GlobalManager.singleton.GetCurrentScene() == "FightScene")
        {
            StopServer();
            GlobalManager.singleton.LoadScene("SelectionScene");
        }
        else
        {
            playersReady = Math.Max(playersReady - 1, 0);
        }
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        if (GlobalManager.singleton.GetCurrentScene() == "FightScene")
        {
            GlobalManager.singleton.LoadScene("SelectionScene");
        }
    }
}
