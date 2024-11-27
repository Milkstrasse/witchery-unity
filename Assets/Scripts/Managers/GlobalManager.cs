using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager singleton;

    [SerializeField] private Material[] materials;

    public bool isConnected;
    public GameMode mode;
    public string joincode;
    public bool relayEnabled;
    public int maxPlayers;
    public string lastScene;

    public event Action<string> OnCodeCreated;
    
    private async void Awake()
    {
        DontDestroyOnLoad(this);
        singleton = this;

        GlobalData.fighters = Resources.LoadAll<Fighter>("Fighters/");
        Array.Sort(GlobalData.fighters, (a,b) => { return a.fighterID.CompareTo(b.fighterID); });
        GlobalData.missions = Resources.LoadAll<Mission>("Missions/");
        GlobalData.themes = Resources.LoadAll<Theme>("Themes/");

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
        GlobalData.highlightPlayable = PlayerPrefs.GetInt("highlightPlayable", 0) != 0;
        GlobalData.animateImpact = PlayerPrefs.GetInt("animateImpact", 0) != 0;
        GlobalData.themeIndex = PlayerPrefs.GetInt("theme", 1);

        ApplyTheme();

        #if UNITY_EDITOR
            relayEnabled = false;
        #endif

        if (SaveManager.LoadData())
        {
            Debug.Log("Welcome " + SaveManager.savedData.name);
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
            case 1: //damage
                for (int i = 0; i < GlobalData.fighters.Length; i++)
                {
                    if (GlobalData.fighters[i].role == Role.damage)
                    {
                        filteredFighters.Add(i);
                    }
                }

                break;
            case 2: //control
                for (int i = 0; i < GlobalData.fighters.Length; i++)
                {
                    if (GlobalData.fighters[i].role == Role.control)
                    {
                        filteredFighters.Add(i);
                    }
                }

                break;
            case 3: //recovery
                for (int i = 0; i < GlobalData.fighters.Length; i++)
                {
                    if (GlobalData.fighters[i].role == Role.recovery)
                    {
                        filteredFighters.Add(i);
                    }
                }

                break;
            default:
                return Enumerable.Range(0, GlobalData.fighters.Length).ToArray();
        }

        return filteredFighters.ToArray();
    }

    public void ApplyTheme()
    {
        for (int i = 0; i < GlobalData.themes[GlobalData.themeIndex].colors.Length; i++)
        {
            materials[i].color = GlobalData.themes[GlobalData.themeIndex].colors[i];
        }

        TMP_Settings.defaultStyleSheet = GlobalData.themes[GlobalData.themeIndex].sheet;
        TMP_Settings.defaultStyleSheet.RefreshStyles();
    }

    public void GoToMenu()
    {
        SaveManager.CreateNewData(GlobalData.fighters, GlobalData.missions, Random.Range(0, GlobalData.fighters.Length));
        
        LoadScene("MenuScene");
    }

    public string GetCurrentScene() => SceneManager.GetActiveScene().name;

    public void LoadScene(string scene)
    {
        string temp = GetCurrentScene();
        if (temp != scene)
        {
            lastScene = temp;
        }
        
        SceneManager.LoadScene(scene);
    }

    public int[] GetRandomNumbers(int amount, int max)
    {
        int[] numbers = Enumerable.Repeat(-1, amount).ToArray();

        while (amount > 0)
        {
            int random = Random.Range(0, max);
            if (!numbers.Contains(random))
            {
                numbers[amount - 1] = random;
                amount--;
            }
        }

        return numbers;
    }

    public static void QuitAnyConnection()
    {
        NetworkClient.Shutdown();
        NetworkServer.Shutdown();
    }
}

public enum GameMode
{
    Online, Offline, Training, Testing
}