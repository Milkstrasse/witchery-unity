using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MissionOptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private LocalizeStringEvent description;
    [SerializeField] private LocalizeStringEvent claimText;
    [SerializeField] private TextMeshProUGUI reward;

    public Button claimButton;

    public void SetupUI(Mission mission, bool claimed)
    {
        title.text = mission.name;
        reward.text = mission.reward.ToString();

        claimButton.interactable = mission.isClaimable;
        
        if (claimed)
        {
            Claim();
        }
    }

    public void Claim()
    {
        claimButton.interactable = false;
        reward.gameObject.SetActive(false);

        claimText.StringReference.SetReference("StringTable", "claimed");
    }
}
