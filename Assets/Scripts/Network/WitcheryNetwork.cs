using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utp;

public class WitcheryNetwork : RelayNetworkManager
{
    private static int namesUpdated;

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<TeamMessage>(OnReceiveMessage);
    }

    private void OnReceiveMessage(NetworkConnectionToClient conn, TeamMessage message)
    {
        Debug.Log("WitcheryNetwork - RECEIVED A MESSAGE");

        conn.identity.gameObject.GetComponent<Player>().SetupPlayer(message);

        namesUpdated++;

        if (numPlayers == namesUpdated)
        {
            autoCreatePlayer = false;
            ServerChangeScene("FightScene");
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == "FightScene")
        {
            GameObject fightManager = Instantiate(spawnPrefabs[0]);
            NetworkServer.Spawn(fightManager);
        }
    }

    //2 players are needed
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        if (SceneManager.GetActiveScene().name == "FightScene")
        {
            GlobalManager.singleton.LoadScene("MenuScene");
        }

        StopServer();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        if (SceneManager.GetActiveScene().name == "FightScene")
        {
            GlobalManager.singleton.LoadScene("MenuScene");
        }

        StopServer();
    }

    public override void OnStopServer()
    {
        namesUpdated = 0;
        autoCreatePlayer = true;

        base.OnStopServer();
    }
}
