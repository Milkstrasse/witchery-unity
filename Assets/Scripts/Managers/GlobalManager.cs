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
    public StatusEffect[] effects;
    public string joincode;

    public static int waitTime = 60;
    public static int turnTime = 60;

    public bool relayEnabled;
    
    private async void Awake()
    {
        DontDestroyOnLoad(this);
        singleton = this;

        //Load some data (e.g. language)

        fighters = Resources.LoadAll<Fighter>("Fighters/");
        Array.Sort(fighters, (a,b) => { return a.fighterID.CompareTo(b.fighterID); });

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        LoadScene("MenuScene");
    }

    public void LoadScene(string scene) => SceneManager.LoadScene(scene);

    public static void QuitAnyConnection()
    {
        NetworkClient.Shutdown();
        NetworkServer.Shutdown();
    }
}
