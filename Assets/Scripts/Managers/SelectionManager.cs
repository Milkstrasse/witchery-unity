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

   public event Action<int> OnTimerChanged;
   private List<SelectedFighter> fighterIDs;
   private string[] playerNames;

   public event Action OnPlayersReady;

   private void Start()
   {
      networkManager = GameObject.Find("NetworkManager").GetComponent<RelayNetworkManager>();
      networkManager.maxConnections = GlobalManager.singleton.maxPlayers;

      fighterIDs = new List<SelectedFighter>();
      playerNames = new string[]{"Player", "Player"};
      isReady = new bool[2];

      NetworkClient.ReplaceHandler<TurnMessage>(PlayersReady);
   }

   public bool SetReady(int index)
   {
      if (fighterIDs.Count == 0)
         return index == 0;

      isReady[index] = !isReady[index];

      if (!isReady[index])
      {
         if (GlobalManager.singleton.mode == GameMode.Online)
         {
            StopAllCoroutines();
            GlobalManager.QuitAnyConnection();
         }

         return false;
      }

      if (GlobalManager.singleton.mode == GameMode.Online)
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
         //GlobalManager.singleton.leaders[index] = fighterIDs[0];

         fighterIDs = new List<SelectedFighter>();

         if (GlobalManager.singleton.mode == GameMode.Training)
         {
            fighterIDs.Add(new SelectedFighter(0, 0));
            fighterIDs.Add(new SelectedFighter(1, 0));

            NetworkClient.Send(new PlayerMessage(playerNames[index], fighterIDs.ToArray()));
            fighterIDs = new List<SelectedFighter>();
         }
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
         OnTimerChanged?.Invoke(time);

         if (NetworkClient.isConnected && !sentMessage)
         {
            NetworkClient.Send(new PlayerMessage(playerNames[1], fighterIDs.ToArray()));
            //GlobalManager.singleton.leaders[1] = fighterIDs[0];

            sentMessage = true;
         }
         
         yield return new WaitForSeconds(1.0f);
      }

      GlobalManager.QuitAnyConnection();
      ReturnToMenu();
   }

   public SelectionResult EditTeam(SelectedFighter fighter)
   {
      int index = fighterIDs.IndexOf(fighter);

      if (index >= 0)
      {
         fighterIDs.RemoveAt(index);
         bool hasTeam = fighterIDs.Count > 0;

         return new SelectionResult(false, hasTeam);
      }
      else if (fighterIDs.Count < 6)
      {
         fighterIDs.Add(fighter);
         bool hasTeam = fighterIDs.Count > 0;
         
         return new SelectionResult(true, hasTeam);
      }
      else
      {
         return new SelectionResult(false, true);
      }
   }

   private void PlayersReady(TurnMessage message)
   {
      OnPlayersReady?.Invoke();
   }

   public void ReturnToMenu()
   {
      AudioManager.singleton.PlayStandardSound();
      GlobalManager.singleton.LoadScene("MenuScene");
   }
}
