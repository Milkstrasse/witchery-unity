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

        GlobalManager.singleton.StoreRelayCode(relayJoinCode);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        NetworkClient.RegisterHandler<PlayerMessage>(OnClientReceivePlayer);
    }

    private void OnServerReceivePlayer(NetworkConnectionToClient conn, PlayerMessage message)
    {
        message.name = $"PLAYER {playersReady}";
        StatusEffect effect = new StatusEffect(GlobalManager.singleton.fighters[message.fighterIDs[0]].effect);
        message.effects = new StatusEffect[1] {effect};

        players.Add(message);

        playersReady++;

        if (playersReady == 2)
        {
            StatusEffect effect0 = new StatusEffect(GlobalManager.singleton.fighters[players[0].fighterIDs[0]].effect);
            StatusEffect effect1 = new StatusEffect(GlobalManager.singleton.fighters[players[1].fighterIDs[0]].effect);

            if (effect0.value < 0 && effect1.value < 0)
            {
                players[0] = new PlayerMessage(players[0].name, players[0].fighterIDs, new StatusEffect[1] {effect1});
                players[1] = new PlayerMessage(players[1].name, players[1].fighterIDs, new StatusEffect[1] {effect0});
            }
            else if (effect0.value >= 0 && effect1.value < 0)
            {
                players[0] = new PlayerMessage(players[0].name, players[0].fighterIDs, new StatusEffect[2] {effect0, effect1});
                players[1] = new PlayerMessage(players[1].name, players[1].fighterIDs, new StatusEffect[0]);
            }
            else if (effect0.value < 0 && effect1.value >= 0)
            {
                players[0] = new PlayerMessage(players[0].name, players[0].fighterIDs, new StatusEffect[0]);
                players[1] = new PlayerMessage(players[1].name, players[1].fighterIDs, new StatusEffect[2] {effect0, effect1});
            }

            playersReady = 0;
            NetworkServer.SendToAll(new TurnMessage());

            Invoke("ChangeToFightScene", 0.3f);
        }
    }

    private void ChangeToFightScene()
    {
        ServerChangeScene("FightScene");
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
