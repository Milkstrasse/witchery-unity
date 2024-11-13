using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;
    [SerializeField] private RectTransform shopRect;
    private ShopOptionUI[] options;
    [SerializeField] private LocalizeStringEvent shopTitle;
    [SerializeField] private TextMeshProUGUI refresh;
    private int refreshOutfits;
    private int refreshFighters;
    private bool shwowingFighters;

    private void Start()
    {
        manager.OnShopOptionsCreated += SetupShopOptions;

        options = shopRect.transform.GetComponentsInChildren<ShopOptionUI>();
    }

    private void SetupShopOptions(SelectedFighter[] fighters, int startIndex)
    {
        if (fighters.Length < 6)
        {
            if (startIndex == 0)
            {
                refreshOutfits = 0;
            }
            else
            {
                refreshFighters = 0;
            }
        }

        for (int i = 0; i < fighters.Length; i++)
        {
            options[startIndex + i].SetupUI(GlobalData.fighters[fighters[i].fighterID], fighters[i].outfit);
            
            if (fighters.Length < 6)
            {
                if (options[i].button.interactable && i < 3 && startIndex < 3)
                {
                    refreshOutfits += 40;
                }
                else if (options[startIndex + i].button.interactable && i < 3 && startIndex > 0)
                {
                    refreshFighters += 40;
                }
            }
            else
            {
                if (i < 3 && options[i].button.interactable)
                {
                    refreshOutfits += 40;
                }
                else if (options[i].button.interactable)
                {
                    refreshFighters += 40;
                }
            }
        }

        if (shwowingFighters)
        {
            refresh.text = $"{refreshFighters} SP";
        }
        else
        {
            refresh.text = $"{refreshOutfits} SP";
        }
    }

    public void UnlockOutfit(int index)
    {
        if (manager.UnlockOutfit(options[index].fighter, options[index].outfit))
        {
            AudioManager.singleton.PlayPositiveSound();

            options[index].button.interactable = false;

            refreshOutfits -= 40;
            refresh.text = $"{refreshOutfits} SP";
        }
    }

    public void RefreshShop()
    {
        if (shwowingFighters)
        {
            if (SaveManager.savedData.money >= refreshFighters)
            {
                AudioManager.singleton.PlayStandardSound();

                SaveManager.savedData.money -= refreshFighters;
                SaveManager.savedData.moneySpent += refreshFighters;

                manager.OnMoneyChanged.Invoke(SaveManager.savedData.money);
                manager.CreateShopOptions(3, 3);
            }
        }
        else
        {
            if (SaveManager.savedData.money >= refreshOutfits)
            {
                AudioManager.singleton.PlayStandardSound();

                SaveManager.savedData.money -= refreshOutfits;
                SaveManager.savedData.moneySpent += refreshOutfits;

                manager.OnMoneyChanged.Invoke(SaveManager.savedData.money);
                manager.CreateShopOptions(3, 0);
            }
        }

        SaveManager.SaveData();
    }

    public void ToggleShop()
    {
        AudioManager.singleton.PlayStandardSound();

        shwowingFighters = !shwowingFighters;

        if (shwowingFighters)
        {
            LeanTween.moveLocalX(shopRect.gameObject, -shopRect.sizeDelta.x/2f - 10f, 0.3f);
            refresh.text = $"{refreshFighters} SP";

            shopTitle.StringReference.SetReference("StringTable", "fighters");
        }
        else
        {
            LeanTween.moveLocalX(shopRect.gameObject, 0f, 0.3f);
            refresh.text = $"{refreshOutfits} SP";

            shopTitle.StringReference.SetReference("StringTable", "outfits");
        }
    }

    public void ReturnToMenu(MenuUI menuUI)
    {
        AudioManager.singleton.PlayStandardSound();

        SaveManager.SaveData();

        shwowingFighters = false;
        shopRect.localPosition = new Vector3(0f, shopRect.localPosition.y, shopRect.localPosition.z);

        menuUI.SwitchToMainMenu(gameObject);
    }

    private void OnDestroy()
    {
        manager.OnShopOptionsCreated -= SetupShopOptions;
    }
}
