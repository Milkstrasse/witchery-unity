using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Utp;

public class SelectionManager : MonoBehaviour
{
   private RelayNetworkManager networkManager;
   private bool[] isReady;

   public event Action<int> onTimerChanged;
   private List<int> fighterIDs;
   private string[] playerNames;

   private void Start()
   {
      networkManager = GameObject.Find("NetworkManager").GetComponent<RelayNetworkManager>();
      networkManager.maxConnections = GlobalManager.singleton.maxPlayers;

      fighterIDs = new List<int>();
      playerNames = new string[]{"Player", "Player"};
      isReady = new bool[2];
   }

   public bool SetReady(int index)
   {
      if (fighterIDs.Count == 0)
         return false;

      isReady[index] = !isReady[index];

      if (!isReady[index])
      {
         if (GlobalManager.singleton.maxPlayers > 1)
         {
            StopAllCoroutines();
            GlobalManager.QuitAnyConnection();
         }

         return false;
      }

      if (GlobalManager.singleton.maxPlayers > 1)
      {
         StartCoroutine(StartConnection());
      }
      else
      {
         if (!NetworkServer.active)
         {
            networkManager.StartStandardHost();
         }

         NetworkClient.Send(new PlayerMessage(playerNames[index], fighterIDs.ToArray()));
         fighterIDs = new List<int>();
      }

      return isReady[index];
   }

   IEnumerator StartConnection()
   {
      if (GlobalManager.singleton.joincode == "" || networkManager.maxConnections < 2) //start host
      {
         if (GlobalManager.singleton.relayEnabled && networkManager.maxConnections == 2)
         {
            networkManager.StartRelayHost(networkManager.maxConnections);
         }
         else
         {
            networkManager.StartStandardHost();
         }
      }
      else //start client
      {
         if (GlobalManager.singleton.relayEnabled)
         {
            networkManager.relayJoinCode = GlobalManager.singleton.joincode;
            networkManager.JoinRelayServer();
         }
         else
         {
            networkManager.JoinStandardServer();
         }
      }

      int time = GlobalManager.waitTime;
      bool sentMessage = false;

      while (time >= 0)
      {
         time--;
         onTimerChanged?.Invoke(time);

         if (NetworkClient.isConnected && !sentMessage)
         {
            NetworkClient.Send(new PlayerMessage(playerNames[1], fighterIDs.ToArray()));
            sentMessage = true;
         }
         
         yield return new WaitForSeconds(1f);
      }

      GlobalManager.QuitAnyConnection();
      ReturnToMenu();
   }

   public bool EditTeam(int fighterID)
   {
      int index = fighterIDs.IndexOf(fighterID);

      if (index >= 0)
      {
         fighterIDs.RemoveAt(index);
         return false;
      }
      else
      {
         fighterIDs.Add(fighterID);
         return true;
      }
   }

   public void ReturnToMenu() => GlobalManager.singleton.LoadScene("MenuScene");
}
