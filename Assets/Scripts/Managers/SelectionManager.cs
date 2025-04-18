using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Utp;

public class SelectionManager : MonoBehaviour
{
   [SerializeField] private SettingsManager settings;
   [SerializeField] private RelayCode relayCode;

   private RelayNetworkManager networkManager;
   private bool[] isReady;

   public event Action<int> OnTimerChanged;
   private List<SelectedFighter> fighterIDs;

   public event Action OnPlayersReady;

   private void Awake()
   {
      networkManager = GameObject.Find("NetworkManager").GetComponent<RelayNetworkManager>();
      networkManager.maxConnections = GlobalManager.singleton.maxPlayers;

      fighterIDs = new List<SelectedFighter>();
      isReady = new bool[2];

      NetworkClient.ReplaceHandler<TurnMessage>(PlayersReady);

      CheckMissions();

      if (GlobalManager.singleton.mode != GameMode.Online)
      {
         GlobalManager.singleton.mode = GameMode.Offline;
      }
   }

   public void SetAsRematch()
   {
      relayCode.ToggleRematch(true);
   }

   public bool SetReady(int index)
   {
      if (fighterIDs.Count == 0)
         return index == 0;

      if (index == 1)
      {
         relayCode.SetInteractable(isReady[index]);
      }

      isReady[index] = !isReady[index];

      if (!isReady[index])
      {
         if (GlobalManager.singleton.mode == GameMode.Online)
         {
            StopAllCoroutines();

            GlobalManager.QuitAnyConnection();
            NetworkClient.ReplaceHandler<TurnMessage>(PlayersReady);
         }

         relayCode.ToggleRematch(false);

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
      }

      return isReady[index];
   }

   public bool ToggleCPU()
   {
      if (GlobalManager.singleton.mode == GameMode.Offline)
      {
         GlobalManager.singleton.mode = GameMode.Training;
         return true;
      }
      else
      {
         GlobalManager.singleton.mode = GameMode.Offline;
         return false;
      }
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

   public SelectionResult EditTeam(int fighter, int outfit)
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

      return new SelectionResult(false, true, fighterIDs[0]);
   }

   private void PlayersReady(TurnMessage message)
   {
      OnPlayersReady?.Invoke();
      NetworkClient.UnregisterHandler<TurnMessage>();

      LeanTween.moveLocalY(relayCode.gameObject, 0f, 0.3f);
   }

   public void CheckMissions()
   {
      for (int i = 0; i < GlobalData.missions.Length; i++)
      {
         GlobalData.missions[i].CheckStatus(i);
      }
   }

   public void DeleteData()
   {
      AudioManager.singleton.PlayNegativeSound();

      SaveManager.DeleteData();
      SaveManager.CreateNewData(GlobalData.fighters, GlobalData.missions);

      CheckMissions();

      GlobalManager.singleton.LoadScene("SelectionScene");
   }

   public void UnlockFighters()
   {
      AudioManager.singleton.PlayPositiveSound();

      for (int i = 0; i < GlobalData.fighters.Length; i++)
      {
         SaveManager.savedData.fighters[i].UnlockFighter();
      }

      SaveManager.SaveData();

      CheckMissions();

      GlobalManager.singleton.LoadScene("SelectionScene");
   }

   public void ToggleSettings(bool enable)
   {
      AudioManager.singleton.PlayStandardSound();

      if (!enable)
      {
         if (settings.SavingSettings())
         {
            settings.gameObject.SetActive(false);
         }
      }
      else
      {
         settings.gameObject.SetActive(true);
      }
   }
}