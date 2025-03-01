using TMPro;
using UnityEngine;

public class FightLogUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI log;

    private void OnEnable()
    {
        log.text = GlobalManager.singleton.fightLog.GetLog();
    }
}
