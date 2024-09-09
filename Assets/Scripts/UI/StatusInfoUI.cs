using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class StatusInfoUI : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent infoText;
    [SerializeField] private LocalizeStringEvent turns;
    [SerializeField] private TextMeshProUGUI icon;

    public void SetupInfo(StatusEffect effect, int index)
    {
        uint i = Convert.ToUInt32(effect.icon, 16);
        icon.text = Convert.ToChar(i).ToString();

        infoText.StringReference.SetReference("StringTable", effect.name + "Descr");
        infoText.RefreshString();

        (turns.StringReference["duration"] as IntVariable).Value = effect.duration;
        turns.RefreshString();
    }
}
