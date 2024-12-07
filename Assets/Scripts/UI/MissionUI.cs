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

        toClaim = 0;
        missions = new MissionOptionUI[GlobalData.missions.Length];

        for (int i = 0; i < GlobalData.missions.Length; i++)
        {
            MissionOptionUI mission = Instantiate(missionPrefab, transform).GetComponent<MissionOptionUI>();

            int iCopy = i;
            mission.claimButton.onClick.AddListener(() => ClaimMission(iCopy));

            missions[i] = mission;
        }

        RectTransform rectTransform = transform as RectTransform;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 100 * GlobalData.missions.Length + (GlobalData.missions.Length - 1) * 10f);
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

        //bookmark.SetActive(toClaim != 0);
    }

    private void ClaimMission(int index)
    {
        if (manager.ClaimMission(index))
        {
            AudioManager.singleton.PlayPositiveSound();

            missions[index].Claim();

            toClaim--;
            //bookmark.SetActive(toClaim != 0);
        }
    }

    private void OnDestroy()
    {
        manager.OnMissionsUpdated -= UpdateMissions;
    }
}
