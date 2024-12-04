using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Utp;

public class SelectionManager : MonoBehaviour
{
   private RelayNetworkManager networkManager;
   private bool[] isReady;

   public event Action<int> OnTimerChanged;
   private List<SelectedFighter> fighterIDs;
   private string[] playerNames;

   public event Action OnPlayersReady;
   //private bool readyToFight;

   private void Awake()
   {
      networkManager = GameObject.Find("NetworkManager").GetComponent<RelayNetworkManager>();
      networkManager.maxConnections = GlobalManager.singleton.maxPlayers;

      fighterIDs = new List<SelectedFighter>();
      playerNames = new string[2]{LocalizationSettings.StringDatabase.GetLocalizedString("StringTable", "player"), SaveManager.savedData.name};
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
         if (!NetworkClient.active)
         {
            StartCoroutine(StartConnection());
         }
         else
         {
            NetworkClient.Send(new PlayerMessage(fighterIDs.ToArray()));
         }
      }
      else
      {
         if (!NetworkServer.active)
         {
            networkManager.StartStandardHost();
         }

         NetworkClient.Send(new PlayerMessage(fighterIDs.ToArray()));

         int fighterAmount = fighterIDs.Count;
         fighterIDs = new List<SelectedFighter>();

         if (GlobalManager.singleton.mode == GameMode.Training)
         {
            int[] numbers = GlobalManager.singleton.GetRandomNumbers(fighterAmount, GlobalData.fighters.Length);
            for (int i = 0; i < fighterAmount; i++)
            {
               fighterIDs.Add(new SelectedFighter(numbers[i], 0));
            }

            NetworkClient.Send(new PlayerMessage(fighterIDs.ToArray()));
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

      int time = GlobalData.waitTime;
      bool sentMessage = false;

      while (time >= 0)
      {
         time--;
         OnTimerChanged?.Invoke(time);

         if (NetworkClient.isConnected && !sentMessage)
         {
            NetworkClient.Send(new PlayerMessage(fighterIDs.ToArray()));
            sentMessage = true;
         }
         
         yield return new WaitForSeconds(1.0f);
      }

      GlobalManager.QuitAnyConnection();
      ReturnToMenu();
   }

   public SelectionResult EditTeam(SelectedFighter fighter)
   {
      int index = -1;
      for (int i = 0; i < fighterIDs.Count; i++)
      {
         if (fighterIDs[i].fighterID == fighter.fighterID)
         {
            index = i;
            break;
         }
      }

      if (index >= 0)
      {
         fighterIDs.RemoveAt(index);
         bool hasTeam = fighterIDs.Count > 0;

         return new SelectionResult(false, hasTeam, hasTeam ? fighterIDs[0] : new SelectedFighter(0, 0));
      }
      else if (fighterIDs.Count < 6)
      {
         fighterIDs.Add(fighter);
         
         return new SelectionResult(true, true, fighterIDs[0]);
      }
      else
      {
         return new SelectionResult(false, true, fighterIDs[0]);
      }
   }

   public void EditTeam(int fighter, int outfit)
   {
      int index = -1;
      for (int i = 0; i < fighterIDs.Count; i++)
      {
         if (fighterIDs[i].fighterID == fighter)
         {
            index = i;
            break;
         }
      }

      if (index >= 0)
      {
         fighterIDs[index] = new SelectedFighter(fighter, outfit);
      }
   }

   private void PlayersReady(TurnMessage message)
   {
      /*if (readyToFight)
         return;*/
      
      OnPlayersReady?.Invoke();
      NetworkClient.UnregisterHandler<TurnMessage>();
      //readyToFight = true;
   }

   public void ReturnToMenu()
   {
      AudioManager.singleton.PlayStandardSound();
      GlobalManager.singleton.LoadScene("MenuScene");
   }
}
