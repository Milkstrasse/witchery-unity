using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MissionOptionUI : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent title;
    [SerializeField] private LocalizeStringEvent description;
    [SerializeField] private TextMeshProUGUI reward;
    [SerializeField] private Button claimButton;

    public void SetupUI()
    {
        
    }
}
