using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI money;

    [SerializeField] private RectTransform mainMenu;
    [SerializeField] private RectTransform modeMenu;

    [SerializeField] private Button joinButton;
    [SerializeField] private Button hostButton;

    private void Start()
    {
        manager.OnMoneyChanged += UpdatePlayer;

        icon.sprite = Resources.Load<Sprite>("Sprites/" + GlobalManager.singleton.fighters[SaveManager.savedData.icon].name + "-standard");
        playerName.text = SaveManager.savedData.name;
        money.text = $"{SaveManager.savedData.money} SP";

        joinButton.interactable = GlobalManager.singleton.isConnected;
        hostButton.interactable = GlobalManager.singleton.isConnected;

        if (GlobalManager.singleton.maxPlayers > 0)
        {
            mainMenu.localPosition = new Vector3(-mainMenu.sizeDelta.x * 1.5f - 20f, mainMenu.localPosition.y, mainMenu.localPosition.z);
            modeMenu.localPosition = new Vector3(-mainMenu.sizeDelta.x * 0.5f, modeMenu.localPosition.y, modeMenu.localPosition.z);
        }
    }

    public void SwitchToModeMenu()
    {
        AudioManager.singleton.PlayStandardSound();

        GlobalManager.singleton.maxPlayers = 5;

        LeanTween.moveLocalX(mainMenu.gameObject, -mainMenu.sizeDelta.x * 1.5f - 20f, 0.3f);
        LeanTween.moveLocalX(modeMenu.gameObject, -mainMenu.sizeDelta.x * 0.5f, 0.3f);
    }

    public void SwitchToMenu(GameObject newMenu)
    {
        AudioManager.singleton.PlayStandardSound();

        LeanTween.moveLocalX(mainMenu.gameObject, -mainMenu.sizeDelta.x * 1.5f - 20f, 0.3f);
        LeanTween.moveLocalX(newMenu, -mainMenu.sizeDelta.x * 0.5f, 0.3f);
    }

    public void SwitchToMainMenu(GameObject currMenu)
    {
        AudioManager.singleton.PlayStandardSound();
        
        GlobalManager.singleton.maxPlayers = 0;

        LeanTween.moveLocalX(mainMenu.gameObject, -mainMenu.sizeDelta.x * 0.5f, 0.3f);
        LeanTween.moveLocalX(currMenu, mainMenu.sizeDelta.x * 0.5f + 20f, 0.3f);
    }

    private void UpdatePlayer(int money)
    {
        this.money.text = $"{money} SP";
    }

    private void OnDestroy()
    {
        manager.OnMoneyChanged -= UpdatePlayer;
    }
}
