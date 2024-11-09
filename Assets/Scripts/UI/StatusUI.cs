using System;
using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private StatusInfoUI info;

    [SerializeField] private TextMeshProUGUI icon;

    private StatusEffect effect;
    private string iconString;

    public void SetupEffect(StatusEffect effect)
    {
        this.effect = effect;

        uint i = Convert.ToUInt32(effect.icon, 16);
        icon.text = Convert.ToChar(i).ToString();

        iconString = icon.text;

        if (effect.isNew)
        {
            LeanTween.scale(transform.GetChild(0).gameObject, new Vector3(1.3f, 1.3f, 1.3f), 0.2f).setLoopPingPong(1);
            LeanTween.scale(icon.gameObject, new Vector3(1.3f, 1.3f, 1.3f), 0.2f).setLoopPingPong(1);

            effect.isNew = false;
        }
    }

    public void HideInfo()
    {
        info.gameObject.SetActive(false);
    }

    public void ShowInfo()
    {
        if (effect == null)
        {
            return;
        }

        info.gameObject.SetActive(true);
        info.SetupInfo(effect);
    }
}
