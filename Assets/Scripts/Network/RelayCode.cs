using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class RelayCode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;
    [SerializeField] private LocalizeStringEvent readyText;

    private void Start()
    {
        if (GlobalManager.singleton.isConnected)
        {
            codeText.text = "";
            GlobalManager.singleton.OnCodeCreated += ShowCode;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetJoincode(string joincode)
    {
        GlobalManager.singleton.joincode = joincode;

        if (joincode.Length > 0)
        {
            readyText.StringReference.SetReference("StringTable", "join");
        }
        else
        {
            readyText.StringReference.SetReference("StringTable", "host");
        }
    }

    private void ShowCode(string code)
    {
        codeText.text = code;
    }
}
