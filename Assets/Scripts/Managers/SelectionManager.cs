using Mirror;
using System;
using System.Collections;
using UnityEngine;
using Utp;

public class SelectionManager : MonoBehaviour
{
    [SerializeField]
    private SelectionUI selectionUI;
    
    public Action<bool> onStartedClient;
    public Action<int> onTimerChanged;

    private RelayNetworkManager networkManager;

    private void Awake()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<RelayNetworkManager>();
    }

    public bool SetReady(bool ready, PlayerSelectionUI player)
    {
        if (player.fighterIDs.Count == 0)
            return false;

        if (ready && GlobalManager.singleton.joincode != null && GlobalManager.singleton.joincode.Length > 0)
        {
            Debug.Log("started client");
            networkManager.relayJoinCode = GlobalManager.singleton.joincode;
            if (Application.isEditor)
            {
                networkManager.JoinStandardServer();
            }
            else
            {
                networkManager.JoinRelayServer();
            }
            //NetworkManager.singleton.StartClient();

            onStartedClient?.Invoke(false);

            StartCoroutine(WaitForConnection());

            return true;
        }
        else if (ready)
        {
            Debug.Log("started host");
            if (Application.isEditor)
            {
                networkManager.StartStandardHost();
            }
            else
            {
                networkManager.StartRelayHost(2);
            }
            //NetworkManager.singleton.StartHost();
            onStartedClient?.Invoke(true);

            StartCoroutine(WaitForConnection());

            return true;
        }
        else
        {
            StopAllCoroutines();

            onTimerChanged?.Invoke(GlobalManager.connectionTime);

            NetworkClient.Shutdown();
            NetworkServer.Shutdown();

            return false;
        }
    }

    IEnumerator WaitForConnection()
    {
        bool isClient = true;

        int time = GlobalManager.connectionTime;

        while (time >= 0 && ((NetworkClient.activeHost && NetworkServer.connections.Count == 1) || !NetworkClient.isConnected))
        {
            time -= 1;
            onTimerChanged?.Invoke(time);

            if (isClient && !NetworkClient.active)
            {
                Debug.Log("stopped being a client");
                NetworkManager.singleton.StopClient(); //necessary?
                isClient = false;
            }

            yield return new WaitForSeconds(1);
        }

        if ((NetworkServer.active && NetworkServer.connections.Count == 2) || (!NetworkClient.activeHost && NetworkClient.isConnected))
        {
            StartFight(); 
        }
        else
        {
            StopSelection();
        }
    }

    public void StopSelection()
    {
        GlobalManager.singleton.LoadScene("MenuScene");
    }

    public void StartFight()
    {
        selectionUI.playerBottom.StartFight();
        TeamMessage msg = new TeamMessage {
            name = GlobalManager.teamName,
            fighterIDs = selectionUI.playerBottom.fighterIDs.ToArray()
        };

        NetworkClient.Send(msg);
    }
}