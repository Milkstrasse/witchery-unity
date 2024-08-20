using System;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class MenuUI : MonoBehaviour
{   
    [SerializeField]
    private LocalizeStringEvent stringEvent;
    private int currLocale;
    private bool changingLocale;

    private void Start()
    {
        currLocale = 0;

        NetworkClient.Shutdown();
        NetworkServer.Shutdown();

        GlobalManager.singleton.joincode = "";
    }

    public void SetJoincode(string joincode)
    {
        GlobalManager.singleton.joincode = joincode;
        if (joincode == null || joincode == "")
        {
            stringEvent.StringReference.SetReference("StringTable", "host");
        }
        else
        {
            stringEvent.StringReference.SetReference("StringTable", "join");
        }
    }

    public void StartSelection()
    {
        GlobalManager.singleton.LoadScene("SelectionScene");
    }

    public void ChangeLocale()
    {
        if (changingLocale)
        {
            return;
        }

        if (currLocale < LocalizationSettings.AvailableLocales.Locales.Count - 1)
        {
            currLocale++;
        }
        else
        {
            currLocale = 0;
        }

        StartCoroutine(SetLocale(currLocale));
    }

    IEnumerator SetLocale(int locale)
    {
        changingLocale = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[locale];

        changingLocale = false;
    }
}