using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public bool UnlockOutfit(Fighter fighter, int outfit)
    {
        int cost = fighter.outfits[outfit].cost;
        if (GlobalSettings.money >= cost && !GlobalSettings.unlocked[fighter.fighterID, outfit])
        {
            GlobalSettings.money -= cost;
            GlobalSettings.unlocked[fighter.fighterID, outfit] = true;

            return true;
        }
        
        return false;
    }

    public void Reset()
    {
        GlobalSettings.money = 500;
        SaveManager.CreateNewData(GlobalManager.singleton.fighters);
    }

    public void ReturnToMenu()
    {
        AudioManager.singleton.PlayStandardSound();
        SaveManager.SaveData();

        GlobalManager.singleton.LoadScene("MenuScene");
    }
}
