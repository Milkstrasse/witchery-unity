using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public event Action<SelectedFighter[], int> OnShopOptionsCreated;
    public Action<int> OnMoneyChanged;

    private void Start()
    {
        GlobalManager.QuitAnyConnection();
        GlobalManager.singleton.joincode = "";

        CreateShopOptions(6, 0);
    }

    public void CreateShopOptions(int amount, int offset)
    {
        SelectedFighter[] options = new SelectedFighter[amount];

        while (amount > 0)
        {
            int fighter = UnityEngine.Random.Range(0, GlobalManager.singleton.fighters.Length);
            
            if (amount + offset > 3)
            {
                options[amount - 1] = new SelectedFighter(fighter, 0);
            }
            else
            {
                options[amount - 1] = new SelectedFighter(fighter, UnityEngine.Random.Range(1, GlobalManager.singleton.fighters[fighter].outfits.Length));
            }

            amount--;
        }

        OnShopOptionsCreated?.Invoke(options, offset);
    }

    public void SetJoincode(string joincode)
    {
        GlobalManager.singleton.joincode = joincode;
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

    public void DeletePlayer()
    {
        SaveManager.DeleteData();
    }

    public bool UnlockOutfit(Fighter fighter, int outfit)
    {
        int cost = fighter.outfits[outfit].cost;
        if (GlobalSettings.money >= cost && !GlobalSettings.unlocked[fighter.fighterID, outfit])
        {
            GlobalSettings.money -= cost;
            GlobalSettings.unlocked[fighter.fighterID, outfit] = true;

            OnMoneyChanged?.Invoke(GlobalSettings.money);

            return true;
        }
        
        return false;
    }
}