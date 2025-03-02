using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class RelayCode : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeText;
    [SerializeField] private LocalizeStringEvent readyText;

    private string hostCode;

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
        if (joincode.Length > 0 && hostCode != joincode)
        {
            GlobalManager.singleton.joincode = joincode;

            readyText.StringReference.SetReference("StringTable", "join");
            readyText.gameObject.name = "join";
        }
        else if (joincode.Length == 0)
        {
            readyText.StringReference.SetReference("StringTable", "host");
            readyText.gameObject.name = "host";

            GlobalManager.singleton.joincode = "";
            hostCode = "";
        }
        else
        {
            GlobalManager.singleton.joincode = "";
            hostCode = "";
        }
    }

    private void ShowCode(string code)
    {
        hostCode = code;
        codeText.text = code;
    }
}
