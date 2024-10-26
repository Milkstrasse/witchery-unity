using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class RelayCode : MonoBehaviour
{
    private TextMeshProUGUI codeText;

    private void Start()
    {
        codeText = GetComponent<TextMeshProUGUI>();
        codeText.text = "";
        GlobalManager.singleton.OnCodeCreated += ShowCode;
    }

    private void ShowCode(string code)
    {
        codeText.text = code;
    }
}
