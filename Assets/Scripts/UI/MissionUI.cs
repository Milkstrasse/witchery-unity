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
        manager.OnMissionsUpdated += UpdateMissions;

        toClaim = 0;
        missions = new MissionOptionUI[GlobalData.missions.Length];

        for (int i = 0; i < GlobalData.missions.Length; i++)
        {
            MissionOptionUI mission = Instantiate(missionPrefab, missionParent).GetComponent<MissionOptionUI>();

            int iCopy = i;
            mission.claimButton.onClick.AddListener(() => ClaimMission(iCopy));

            missions[i] = mission;
        }
    }

    private void UpdateMissions()
    {
        toClaim = 0;

        for (int i = 0; i < missions.Length; i++)
        {
            missions[i].SetupUI(GlobalData.missions[i], SaveManager.savedData.missions[i]);
            if (missions[i].claimButton.interactable)
            {
                toClaim++;
            }
        }

        allButton.interactable = toClaim != 0;
    }

    private void ClaimMission(int index)
    {
        if (manager.ClaimMission(index))
        {
            AudioManager.singleton.PlayPositiveSound();

            missions[index].Claim();

            toClaim--;
            allButton.interactable = toClaim != 0;
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

    private void OnDestroy()
    {
        manager.OnMissionsUpdated -= UpdateMissions;
    }
}
