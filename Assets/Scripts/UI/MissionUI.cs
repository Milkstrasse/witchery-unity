using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;
    [SerializeField] private Transform missionParent;
    [SerializeField] private GameObject missionPrefab;

    private void Start()
    {
        for (int i = 0; i < GlobalData.missions.Length; i++)
        {
            MissionOptionUI mission = Instantiate(missionPrefab, missionParent).GetComponent<MissionOptionUI>();
            mission.SetupUI(GlobalData.missions[i], SaveManager.savedData.missions[i]);

            int iCopy = i;
            mission.claimButton.onClick.AddListener(() => ClaimMission(iCopy, mission));
        }
    }

    private void ClaimMission(int index, MissionOptionUI mission)
    {
        if (manager.ClaimMission(index))
        {
            AudioManager.singleton.PlayPositiveSound();

            mission.Claim();
        }
    }

    public void ReturnToMenu(MenuUI menuUI)
    {
        AudioManager.singleton.PlayStandardSound();

        SaveManager.SaveData();

        menuUI.SwitchToMainMenu(gameObject);
    }
}
