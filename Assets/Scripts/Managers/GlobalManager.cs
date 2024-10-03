using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager singleton;
    public Fighter[] fighters;

    public bool isConnected;
    public GameMode mode;
    public string joincode;
    public bool relayEnabled;
    public int maxPlayers;

    public event Action<string> OnCodeCreated;

    private SaveManager saveManager;
    
    private async void Awake()
    {
        DontDestroyOnLoad(this);
        singleton = this;

        fighters = Resources.LoadAll<Fighter>("Fighters/");
        Array.Sort(fighters, (a,b) => { return a.fighterID.CompareTo(b.fighterID); });

        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            isConnected = true;
            relayEnabled = true;
        }
        catch (Exception exception)
        {
            isConnected = false;
            Debug.LogError(exception);
        }

        AsyncOperationHandle handle = LocalizationSettings.InitializationOperation;
        await handle.Task;

        int langIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("langCode", langIndex)];

        #if UNITY_EDITOR
        relayEnabled = false;
        #endif

        saveManager = new SaveManager();
        if (saveManager.LoadData())
        {
            Debug.Log("Welcome " + GlobalSettings.playerName);
            LoadScene("MenuScene");
        }
        else
        {
            StartManager startManager = GameObject.Find("Canvas").GetComponent<StartManager>();
            startManager.ShowUI();
        }
    }

    public void StoreRelayCode(string code)
    {
        OnCodeCreated?.Invoke(code);
    }

    public int[] GetFighters(int filter)
    {
        List<int> filteredFighters = new List<int>();

        switch (filter)
        {
            case 1: //attack
                for (int i = 0; i < fighters.Length; i++)
                {
                    if (fighters[i].role == Role.attack)
                    {
                        filteredFighters.Add(i);
                    }
                }

                break;
            case 2:
                for (int i = 0; i < fighters.Length; i++)
                {
                    if (fighters[i].role == Role.support)
                    {
                        filteredFighters.Add(i);
                    }
                }

                break;
            default:
                return Enumerable.Range(0, fighters.Length).ToArray();
        }

        return filteredFighters.ToArray();
    }

    public void GoToMenu()
    {
        saveManager.SaveData();
        LoadScene("MenuScene");
    }

    public string GetCurrentScene() => SceneManager.GetActiveScene().name;

    public void LoadScene(string scene) => SceneManager.LoadScene(scene);

    public static void QuitAnyConnection()
    {
        NetworkClient.Shutdown();
        NetworkServer.Shutdown();
    }
}

public enum GameMode
{
    Online, Offline, Training
}