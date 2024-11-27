using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelayCode : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeText;

    private void Start()
    {
        codeText.text = "";
        GlobalManager.singleton.OnCodeCreated += ShowCode;
    }

    private void ShowCode(string code)
    {
        Debug.Log("IIIIIIIII");
        codeText.text = code;
    }
}
