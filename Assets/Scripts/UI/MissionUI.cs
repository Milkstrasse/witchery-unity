using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;
    [SerializeField] private Transform missionParent;
    [SerializeField] private GameObject missionPrefab;
    [SerializeField] private Button allButton;

    private MissionOptionUI[] missions;
    private int toClaim;

    private void Start()
    {
        toClaim = 0;
        missions = new MissionOptionUI[GlobalData.missions.Length];

        for (int i = 0; i < GlobalData.missions.Length; i++)
        {
            MissionOptionUI mission = Instantiate(missionPrefab, missionParent).GetComponent<MissionOptionUI>();
            mission.SetupUI(GlobalData.missions[i], SaveManager.savedData.missions[i]);

            if (mission.claimButton.interactable)
            {
                toClaim++;
            }

            int iCopy = i;
            mission.claimButton.onClick.AddListener(() => ClaimMission(iCopy));

            missions[i] = mission;
        }

        allButton.interactable = toClaim != 0;
    }

    private void ClaimMission(int index)
    {
        if (manager.ClaimMission(index))
        {
            AudioManager.singleton.PlayPositiveSound();

            missions[index].Claim();
        }
    }

    public void ClaimAll()
    {
        toClaim = 0;

        for (int i = 0; i < missions.Length; i++)
        {
            ClaimMission(i);
        }

        allButton.interactable = false;
    }

    public void ReturnToMenu(MenuUI menuUI)
    {
        AudioManager.singleton.PlayStandardSound();

        SaveManager.SaveData();

        menuUI.SwitchToMainMenu(gameObject);
    }
}
