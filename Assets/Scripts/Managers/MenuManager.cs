using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private SettingsManager settings;
    [SerializeField] private Button onlineButton;

    private void Start()
    {
        onlineButton.interactable = UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn;
        
        CheckMissions();
    }

    private void CheckMissions()
    {
        for (int i = 0; i < GlobalData.missions.Length; i++)
        {
            GlobalData.missions[i].CheckStatus(i);
        }
    }

    public void ChangeToSelection(string mode)
    {
        Debug.Log(mode);
        if (Enum.TryParse(mode, out GameMode gameMode))
        {
            GlobalManager.singleton.mode = gameMode;

            if (GlobalManager.singleton.mode == GameMode.Online)
            {
                GlobalManager.singleton.maxPlayers = 2;
            }
            else
            {
                GlobalManager.singleton.maxPlayers = 1;
            }

            AudioManager.singleton.PlayPositiveSound();
            GlobalManager.singleton.LoadScene("SelectionScene");
        }
    }

    public void DeleteData()
    {
        AudioManager.singleton.PlayNegativeSound();

        SaveManager.DeleteData();
        SaveManager.CreateNewData(GlobalData.fighters, GlobalData.missions);

        CheckMissions();

        //GlobalManager.singleton.LoadScene("SelectionScene");
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

        //GlobalManager.singleton.LoadScene("SelectionScene");
    }

    public void ToggleSettings(bool isRotated)
    {
        AudioManager.singleton.PlayStandardSound();
        settings.transform.eulerAngles = new Vector3(0f, 0f, isRotated ? 180f : 0f);

        if (settings.gameObject.activeSelf)
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