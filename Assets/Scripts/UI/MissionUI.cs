using UnityEngine;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;
    [SerializeField] private GameObject missionPrefab;

    private MissionOptionUI[] missions;
    private int toClaim;

    private void Start()
    {
        manager.OnMissionsUpdated += UpdateMissions;
        missions = new MissionOptionUI[GlobalData.missions.Length];

        int nulls = 0;

        for (int i = 0; i < GlobalData.missions.Length; i++)
        {
            if (GlobalData.missions[i].category == Mission.Category.Mission)
            {
                MissionOptionUI mission = Instantiate(missionPrefab, transform).GetComponent<MissionOptionUI>();

                int iCopy = i;
                mission.claimButton.onClick.AddListener(() => ClaimMission(iCopy));

                missions[i] = mission;
            }
            else
            {
                nulls++;
            }
        }

        RectTransform rectTransform = transform as RectTransform;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 100f * (GlobalData.missions.Length - nulls) + (GlobalData.missions.Length - nulls - 1) * 24f);
    }

    private void UpdateMissions()
    {
        toClaim = 0;

        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i] != null)
            {
                missions[i].SetupUI(GlobalData.missions[i], SaveManager.savedData.missions[i]);
                if (missions[i].claimButton.interactable)
                {
                    toClaim++;
                }
            }
        }
    }

    private void ClaimMission(int index)
    {
        if (manager.ClaimMission(index))
        {
            AudioManager.singleton.PlayPositiveSound();

            missions[index].Claim();

            toClaim--;
            manager.missionNotification.SetActive(toClaim == 0);
        }
    }

    private void OnDestroy()
    {
        manager.OnMissionsUpdated -= UpdateMissions;
    }
}
