using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;

    [SerializeField] private Transform header;
    [SerializeField] private Transform headerImage;
    [SerializeField] private TextMeshProUGUI money;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform[] menus;
    [SerializeField] private Image[] buttons;
    [SerializeField] private Material neutral;
    [SerializeField] private Material highlighted;

    private int currMenu;

    private void Start()
    {
        manager.OnMoneyChanged += UpdatePlayer;

        headerImage.position = new Vector3(headerImage.position.x, header.position.y, headerImage.position.z);
        money.text = $"{SaveManager.savedData.money:n0} SP";

        currMenu = GlobalManager.singleton.maxPlayers;

        for (int i = 0; i < currMenu; i++)
        {
            menus[i].localPosition = new Vector3(-menus[0].sizeDelta.x - 30f, menus[i].localPosition.y, menus[i].localPosition.z);
        }
        menus[currMenu].localPosition = new Vector3(0f, menus[currMenu].localPosition.y, menus[currMenu].localPosition.z);

        if (currMenu != 0)
        {
            buttons[0].material = neutral;
            buttons[currMenu].material = highlighted;
            
            scrollRect.content = menus[currMenu];
        }
    }

    public void SwitchToMenu(int index)
    {
        AudioManager.singleton.PlayStandardSound();

        if (currMenu == index)
            return;

        manager.CheckMissions();

        if (currMenu == 3)
        {
            manager.fighterNotification.SetActive(false);
        }
        else if (currMenu == 4)
        {
            SettingsManager settings = menus[4].GetComponent<SettingsManager>();
            if (!settings.SavingSettings())
            {
                return;
            }
        }
        
        GlobalManager.singleton.maxPlayers = index;

        buttons[currMenu].material = neutral;
        buttons[index].material = highlighted;

        if (currMenu < index)
        {
            menus[index].localPosition = new Vector3(menus[currMenu].sizeDelta.x + 30f, menus[index].localPosition.y, menus[index].localPosition.z);
            LeanTween.moveLocalX(menus[currMenu].gameObject, -menus[currMenu].sizeDelta.x - 30f, 0.3f);
        }
        else
        {
            menus[index].localPosition = new Vector3(-menus[currMenu].sizeDelta.x - 30f, menus[index].localPosition.y, menus[index].localPosition.z);
            LeanTween.moveLocalX(menus[currMenu].gameObject, menus[currMenu].sizeDelta.x + 30f, 0.3f);
        }

        LeanTween.moveLocalX(menus[index].gameObject, 0f, 0.3f);

        currMenu = index;
        scrollRect.content = menus[currMenu];
        scrollRect.verticalNormalizedPosition = 1;
    }

    private void UpdatePlayer(int money)
    {
        this.money.text = $"{money:n0} SP";
        manager.CheckMissions();
    }

    private void OnDestroy()
    {
        manager.OnMoneyChanged -= UpdatePlayer;
    }
}
