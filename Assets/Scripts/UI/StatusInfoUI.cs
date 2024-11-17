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
    [SerializeField] private TextMeshProUGUI icon;

    public void SetupInfo(StatusEffect effect)
    {
        uint i = Convert.ToUInt32(effect.icon, 16);
        icon.text = Convert.ToChar(i).ToString();

        (infoText.StringReference["value"] as IntVariable).Value = Math.Abs(effect.value * effect.multiplier);
        infoText.StringReference.SetReference("StringTable", effect.name);
        infoText.RefreshString();
    }
}
