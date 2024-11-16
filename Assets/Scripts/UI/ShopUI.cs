using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;
    [SerializeField] private RectTransform outfitRect;
    [SerializeField] private RectTransform fighterRect;
    private ShopOptionUI[] options;
    [SerializeField] private LocalizeStringEvent shopTitle;
    [SerializeField] private Button refreshButton;
    [SerializeField] private TextMeshProUGUI buttonText;
    private int refreshOutfits;
    private int refreshFighters;
    private bool shwowingFighters;

    private void Start()
    {
        manager.OnShopOptionsCreated += SetupShopOptions;
        manager.OnMoneyChanged += CheckRefreshStatus;

        options = new ShopOptionUI[6];

        ShopOptionUI[] outfits = outfitRect.transform.GetComponentsInChildren<ShopOptionUI>();
        ShopOptionUI[] fighters = fighterRect.transform.GetComponentsInChildren<ShopOptionUI>();

        Array.Copy(outfits, options, outfits.Length);
        Array.Copy(fighters, 0, options, outfits.Length, fighters.Length);
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
                if (options[i].isUnlockable && i < 3 && startIndex < 3)
                {
                    refreshOutfits += 40;
                }
                else if (options[startIndex + i].isUnlockable && i < 3 && startIndex > 0)
                {
                    refreshFighters += 40;
                }
            }
            else
            {
                if (i < 3 && options[i].isUnlockable)
                {
                    refreshOutfits += 40;
                }
                else if (options[i].isUnlockable)
                {
                    refreshFighters += 40;
                }
            }
        }

        if (shwowingFighters)
        {
            buttonText.text = $"{refreshFighters} SP";
            refreshButton.interactable = refreshFighters <= SaveManager.savedData.money;
        }
        else
        {
            buttonText.text = $"{refreshOutfits} SP";
            refreshButton.interactable = refreshOutfits <= SaveManager.savedData.money;
        }
    }

    public void UnlockOutfit(int index)
    {
        if (manager.UnlockOutfit(options[index].fighter, options[index].outfit))
        {
            AudioManager.singleton.PlayPositiveSound();

            if (shwowingFighters)
            {
                refreshFighters -= 40;
                buttonText.text = $"{refreshFighters} SP";
            }
            else
            {
                refreshOutfits -= 40;
                buttonText.text = $"{refreshOutfits} SP";
            }

            for (int i = 0; i < 6; i++)
            {
                if (options[i].CheckStatus() && i < 3)
                {
                    refreshOutfits += 40;
                }
            }
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

    public void DecreaseOption()
    {
        if (LeanTween.isTweening(outfitRect))
        {
            return;
        }

        AudioManager.singleton.PlayStandardSound();

        shwowingFighters = !shwowingFighters;

        if (shwowingFighters)
        {
            fighterRect.localPosition = new Vector3(-outfitRect.sizeDelta.x - 20f, fighterRect.localPosition.y, fighterRect.localPosition.z);
            LeanTween.moveLocalX(outfitRect.gameObject, outfitRect.sizeDelta.x + 20f, 0.3f);
            LeanTween.moveLocalX(fighterRect.gameObject, 0f, 0.3f);

            buttonText.text = $"{refreshFighters} SP";
            shopTitle.StringReference.SetReference("StringTable", "fighters");

            refreshButton.interactable = refreshFighters <= SaveManager.savedData.money;
        }
        else
        {
            outfitRect.localPosition = new Vector3(-outfitRect.sizeDelta.x - 20f, outfitRect.localPosition.y, outfitRect.localPosition.z);
            LeanTween.moveLocalX(outfitRect.gameObject, 0f, 0.3f);
            LeanTween.moveLocalX(fighterRect.gameObject, outfitRect.sizeDelta.x + 20f, 0.3f);

            buttonText.text = $"{refreshOutfits} SP";
            shopTitle.StringReference.SetReference("StringTable", "outfits");

            refreshButton.interactable = refreshOutfits <= SaveManager.savedData.money;
        }
    }

    public void IncreaseOption()
    {
        if (LeanTween.isTweening(outfitRect))
        {
            return;
        }

        AudioManager.singleton.PlayStandardSound();

        shwowingFighters = !shwowingFighters;

        if (shwowingFighters)
        {
            fighterRect.localPosition = new Vector3(outfitRect.sizeDelta.x + 20f, fighterRect.localPosition.y, fighterRect.localPosition.z);
            LeanTween.moveLocalX(outfitRect.gameObject, -outfitRect.sizeDelta.x - 20f, 0.3f);
            LeanTween.moveLocalX(fighterRect.gameObject, 0f, 0.3f);

            buttonText.text = $"{refreshFighters} SP";
            shopTitle.StringReference.SetReference("StringTable", "fighters");

            refreshButton.interactable = refreshFighters <= SaveManager.savedData.money;
        }
        else
        {
            outfitRect.localPosition = new Vector3(outfitRect.sizeDelta.x + 20f, outfitRect.localPosition.y, outfitRect.localPosition.z);
            LeanTween.moveLocalX(outfitRect.gameObject, 0f, 0.3f);
            LeanTween.moveLocalX(fighterRect.gameObject, -outfitRect.sizeDelta.x - 20f, 0.3f);

            buttonText.text = $"{refreshOutfits} SP";
            shopTitle.StringReference.SetReference("StringTable", "outfits");

            refreshButton.interactable = refreshOutfits <= SaveManager.savedData.money;
        }
    }

    private void CheckRefreshStatus(int money)
    {
        if (shwowingFighters)
        {
            refreshButton.interactable = refreshFighters <= money;
        }
        else
        {
            refreshButton.interactable = refreshOutfits <= money;
        }
    }

    public void ReturnToMenu(MenuUI menuUI)
    {
        AudioManager.singleton.PlayStandardSound();

        SaveManager.SaveData();

        shwowingFighters = false;
        outfitRect.localPosition = new Vector3(0f, outfitRect.localPosition.y, outfitRect.localPosition.z);
        fighterRect.localPosition = new Vector3(outfitRect.sizeDelta.x + 20f, fighterRect.localPosition.y, fighterRect.localPosition.z);

        menuUI.SwitchToMainMenu(gameObject);
    }

    private void OnDestroy()
    {
        manager.OnShopOptionsCreated -= SetupShopOptions;
        manager.OnMoneyChanged -= CheckRefreshStatus;
    }
}
