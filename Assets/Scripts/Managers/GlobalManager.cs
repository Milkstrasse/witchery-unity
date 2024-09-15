using System;
using Mirror;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager singleton;
    public Fighter[] fighters;

    public GameMode mode;
    public string joincode;
    public bool relayEnabled = true;
    public int maxPlayers;

    public static int waitTime = 120;
    public static int turnTime = 180;

    public event Action<string> OnCodeCreated;
    
    private async void Awake()
    {
        DontDestroyOnLoad(this);
        singleton = this;

        fighters = Resources.LoadAll<Fighter>("Fighters/");
        Array.Sort(fighters, (a,b) => { return a.fighterID.CompareTo(b.fighterID); });

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        #if UNITY_EDITOR
        relayEnabled = false;
        #endif

        LoadScene("MenuScene");
    }

    public void StoreRelayCode(string code)
    {
        OnCodeCreated?.Invoke(code);
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