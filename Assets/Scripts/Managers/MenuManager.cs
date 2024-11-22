using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button joinButton;
    
    public event Action<SelectedFighter[], int> OnShopOptionsCreated;
    public event Action OnMissionsUpdated;
    public Action<int> OnMoneyChanged;

    private void Start()
    {
        GlobalManager.QuitAnyConnection();
        GlobalManager.singleton.joincode = "";

        if (SaveManager.savedData.shopFighters.Length > 0)
        {
            OnShopOptionsCreated?.Invoke(SaveManager.savedData.shopFighters, 0);
        }
        else
        {
            CreateShopOptions(6, 0);
        }

        CheckMissions();
    }

    public void CheckMissions()
    {
        for (int i = 0; i < GlobalData.missions.Length; i++)
        {
            GlobalData.missions[i].CheckStatus();
        }

        OnMissionsUpdated?.Invoke();
    }

    public void CreateShopOptions(int amount, int offset)
    {
        int[] numbers = GlobalManager.singleton.GetRandomNumbers(amount, GlobalData.fighters.Length);
        SelectedFighter[] options = new SelectedFighter[amount];

        for (int i = 0; i < amount; i++)
        {
            if (i + offset >= 3)
            {
                options[i] = new SelectedFighter(numbers[i], 0);
            }
            else
            {
                options[i] = new SelectedFighter(numbers[i], UnityEngine.Random.Range(1, GlobalData.fighters[numbers[i]].outfits.Length));
            }
        }

        if (options.Length > 3)
        {
            SaveManager.savedData.shopFighters = options;
        }
        else
        {
            for (int i = 0; i < options.Length; i++)
            {
                SaveManager.savedData.shopFighters[i + offset] = options[i];
            }
        }

        SaveManager.SaveData();

        OnShopOptionsCreated?.Invoke(options, offset);
    }

    public void SetJoincode(string joincode)
    {
        GlobalManager.singleton.joincode = joincode;
        joinButton.interactable = joincode.Length > 0;
    }

    public void StartSelection()
    {
        if (GlobalManager.singleton.joincode == "")
            return;
        
        AudioManager.singleton.PlayStandardSound();

        GlobalManager.singleton.mode = GameMode.Online;
        
        GlobalManager.singleton.maxPlayers = 2;
        GlobalManager.singleton.LoadScene("SelectionScene");
    }

    public void StartSelection(int mode)
    {
        AudioManager.singleton.PlayStandardSound();
        
        GlobalManager.singleton.mode = (GameMode) mode;
        if (mode == 0)
        {
            GlobalManager.singleton.maxPlayers = 2;
        }
        else
        {
            GlobalManager.singleton.maxPlayers = 1;
        }
        
        GlobalManager.singleton.LoadScene("SelectionScene");
    }

    public void GoToScene(string scene)
    {
        AudioManager.singleton.PlayStandardSound();
        GlobalManager.singleton.LoadScene(scene);
    }

    public bool UnlockOutfit(Fighter fighter, int outfit)
    {
        int cost = fighter.outfits[outfit].cost;
        if (SaveManager.savedData.money >= cost && !SaveManager.savedData.unlocked[fighter.fighterID, outfit])
        {
            SaveManager.savedData.money -= cost;
            SaveManager.savedData.unlocked[fighter.fighterID, outfit] = true;
            SaveManager.savedData.moneySpent += cost;

            OnMoneyChanged?.Invoke(SaveManager.savedData.money);

            return true;
        }
        
        return false;
    }

    public bool ClaimMission(int index)
    {
        Mission mission = GlobalData.missions[index];

        if (mission.isClaimable && !SaveManager.savedData.missions[index])
        {
            SaveManager.savedData.money = Math.Min(SaveManager.savedData.money + mission.reward, 999999);
            SaveManager.savedData.missions[index] = true;

            OnMoneyChanged?.Invoke(SaveManager.savedData.money);
            return true;
        }

        return false;
    }
}