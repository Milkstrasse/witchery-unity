using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Utp;

public class SelectionManager : MonoBehaviour
{
   private RelayNetworkManager networkManager;
   private bool isReady;

   public event Action<int> onTimerChanged;
   private List<int> fighterIDs;
   private string playerName;

   private void Start()
   {
      networkManager = GameObject.Find("NetworkManager").GetComponent<RelayNetworkManager>();
      fighterIDs = new List<int>();

      playerName = "Player";
   }

   public bool SetReady()
   {
      if (fighterIDs.Count == 0)
         return false;

      isReady = !isReady;

      if (!isReady)
      {
         StopAllCoroutines();
         GlobalManager.QuitAnyConnection();

         return false;
      }

      StartCoroutine(StartConnection());

      return isReady;
   }

   IEnumerator StartConnection()
   {
      if (GlobalManager.singleton.joincode == "") //start host
      {
         if (GlobalManager.singleton.relayEnabled)
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
            NetworkClient.Send(new PlayerMessage(playerName, fighterIDs.ToArray()));
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
