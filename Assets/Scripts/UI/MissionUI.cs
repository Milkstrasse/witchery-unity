using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;
    [SerializeField] private RectTransform cat1Parent;
    [SerializeField] private RectTransform cat2Parent;
    [SerializeField] private GameObject missionPrefab;
    [SerializeField] private Button allButton;
    [SerializeField] private LocalizeStringEvent category;
    [SerializeField] private GameObject bookmark;

    private MissionOptionUI[] missions;
    private int toClaim;

    private bool shwowingCat2;

    private void Start()
    {
        manager.OnMissionsUpdated += UpdateMissions;

        toClaim = 0;
        missions = new MissionOptionUI[GlobalData.missions.Length];

        for (int i = 0; i < GlobalData.missions.Length; i++)
        {
            MissionOptionUI mission = Instantiate(missionPrefab, GlobalData.missions[i].category == Mission.Category.One ? cat1Parent : cat2Parent).GetComponent<MissionOptionUI>();

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
        bookmark.SetActive(toClaim != 0);
    }

    private void ClaimMission(int index)
    {
        if (manager.ClaimMission(index))
        {
            AudioManager.singleton.PlayPositiveSound();

            missions[index].Claim();

            toClaim--;
            allButton.interactable = toClaim != 0;
            bookmark.SetActive(toClaim != 0);
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
        bookmark.SetActive(false);
    }

    public void DecreaseOption()
    {
        if (LeanTween.isTweening(cat1Parent.parent.gameObject))
        {
            return;
        }
        
        AudioManager.singleton.PlayStandardSound();

        shwowingCat2 = !shwowingCat2;

        if (shwowingCat2)
        {
            cat2Parent.parent.localPosition = new Vector3(cat1Parent.sizeDelta.x + 20f, cat2Parent.parent.localPosition.y, cat2Parent.parent.localPosition.z);
            LeanTween.moveLocalX(cat1Parent.parent.gameObject, -cat1Parent.sizeDelta.x - 20f, 0.3f);
            LeanTween.moveLocalX(cat2Parent.parent.gameObject, 0f, 0.3f);

            category.StringReference.SetReference("StringTable", "cat2");
        }
        else
        {
            cat1Parent.parent.localPosition = new Vector3(cat1Parent.sizeDelta.x + 20f, cat1Parent.parent.localPosition.y, cat1Parent.parent.localPosition.z);
            LeanTween.moveLocalX(cat1Parent.parent.gameObject, 0f, 0.3f);
            LeanTween.moveLocalX(cat2Parent.parent.gameObject, -cat1Parent.sizeDelta.x - 20f, 0.3f);

            category.StringReference.SetReference("StringTable", "cat1");
        }
    }

    public void IncreaseOption()
    {
        if (LeanTween.isTweening(cat1Parent.parent.gameObject))
        {
            return;
        }

        AudioManager.singleton.PlayStandardSound();

        shwowingCat2 = !shwowingCat2;

        if (shwowingCat2)
        {
            cat2Parent.parent.localPosition = new Vector3(-cat1Parent.sizeDelta.x - 20f, cat2Parent.parent.localPosition.y, cat2Parent.parent.localPosition.z);
            LeanTween.moveLocalX(cat1Parent.parent.gameObject, cat1Parent.sizeDelta.x + 20f, 0.3f);
            LeanTween.moveLocalX(cat2Parent.parent.gameObject, 0f, 0.3f);

            category.StringReference.SetReference("StringTable", "cat2");
        }
        else
        {
            cat1Parent.parent.localPosition = new Vector3(-cat1Parent.sizeDelta.x - 20f, cat1Parent.parent.localPosition.y, cat1Parent.parent.localPosition.z);
            LeanTween.moveLocalX(cat1Parent.parent.gameObject, 0f, 0.3f);
            LeanTween.moveLocalX(cat2Parent.parent.gameObject, cat1Parent.sizeDelta.x + 20f, 0.3f);

            category.StringReference.SetReference("StringTable", "cat1");
        }
    }

    public void ReturnToMenu(MenuUI menuUI)
    {
        AudioManager.singleton.PlayStandardSound();

        SaveManager.SaveData();

        shwowingCat2 = false;

        cat1Parent.parent.localPosition = new Vector3(0f, cat1Parent.parent.localPosition.y, cat1Parent.parent.localPosition.z);
        cat2Parent.parent.localPosition = new Vector3(cat1Parent.sizeDelta.x + 20f, cat2Parent.parent.localPosition.y, cat2Parent.parent.localPosition.z);

        category.StringReference.SetReference("StringTable", "cat1");

        menuUI.SwitchToMainMenu(gameObject);
    }

    private void OnDestroy()
    {
        manager.OnMissionsUpdated -= UpdateMissions;
    }
}
