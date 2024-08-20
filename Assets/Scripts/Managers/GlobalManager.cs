using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager singleton;
    public Fighter[] fighters;
    public string joincode;

    public static string teamName;
    public static int connectionTime = 120;
    public static int playTime = 60;

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

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
