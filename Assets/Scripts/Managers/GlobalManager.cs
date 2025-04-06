using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager singleton;

    [SerializeField] private Material[] materials;

    public GameMode mode;
    public string joincode;
    public bool relayEnabled;
    public int maxPlayers;
    public string lastScene;

    public FightLog fightLog;

    public event Action<string> OnCodeCreated;
    
    private async void Awake()
    {
        DontDestroyOnLoad(this);
        singleton = this;

        GlobalData.fighters = Resources.LoadAll<Fighter>("Fighters/");
        Array.Sort(GlobalData.fighters, (a,b) => { return a.fighterID.CompareTo(b.fighterID); });
        GlobalData.missions = Resources.LoadAll<Mission>("Missions/");
        GlobalData.themes = Resources.LoadAll<Theme>("Themes/");

        if (mode == GameMode.Online)
        {
            try
            {
                await UnityServices.InitializeAsync();
                AuthenticationService.Instance.SwitchProfile(Random.Range(0, 1000000).ToString());
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                maxPlayers = 2;
            }
            catch (Exception exception)
            {
                mode = GameMode.Offline;
                maxPlayers = 1;

                relayEnabled = false;

                Debug.LogError(exception);
            }
        }
        else
        {
            mode = GameMode.Offline;
            maxPlayers = 1;

            relayEnabled = false;
        }

        GlobalData.highlightPlayable = PlayerPrefs.GetInt("highlightPlayable", 1) != 0;
        GlobalData.animateImpact = PlayerPrefs.GetInt("animateImpact", 1) != 0;
        GlobalData.uiScale = PlayerPrefs.GetFloat("uiScale", 1f);
        GlobalData.themeIndex = PlayerPrefs.GetInt("theme", 1);

        ApplyTheme();

        if (!SaveManager.LoadData())
        {
            SaveManager.CreateNewData(GlobalData.fighters, GlobalData.missions);
        }

        LoadScene("SelectionScene");
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

    public string GetCurrentScene() => SceneManager.GetActiveScene().name;

    public void LoadScene(string scene, LoadSceneMode sceneMode = LoadSceneMode.Single)
    {
        string temp = GetCurrentScene();
        if (temp != scene)
        {
            lastScene = temp;
        }

        SceneManager.LoadScene(scene, new LoadSceneParameters
        {
            loadSceneMode = sceneMode,
            localPhysicsMode = LocalPhysicsMode.Physics3D
        });
    }

    public AsyncOperation UnloadScene(string scene)
    {
        return SceneManager.UnloadSceneAsync(scene);
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

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }
}

public enum GameMode
{
    Online, Offline, Training
}