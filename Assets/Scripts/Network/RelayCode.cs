using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelayCode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;

    private void Start()
    {
        codeText.text = "";
        GlobalManager.singleton.OnCodeCreated += ShowCode;
    }

    private void ShowCode(string code)
    {
        codeText.text = code;
    }
}
