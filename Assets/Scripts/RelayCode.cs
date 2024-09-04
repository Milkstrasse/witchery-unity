using TMPro;
using UnityEngine;

public class RelayCode : MonoBehaviour
{
    private TextMeshProUGUI codeText;

    private void Start()
    {
        codeText = GetComponent<TextMeshProUGUI>();
        GlobalManager.singleton.OnCodeCreated += ShowCode;
    }

    private void ShowCode(string code)
    {
        codeText.text = code;
    }
}
