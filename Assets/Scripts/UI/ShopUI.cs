using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;
    [SerializeField] private RectTransform outfitRect;
    [SerializeField] private RectTransform fighterRect;
    private ShopOptionUI[] options;
    [SerializeField] private LocalizeStringEvent shopTitle;
    [SerializeField] private TextMeshProUGUI refresh;
    private int refreshOutfits;
    private int refreshFighters;
    private bool shwowingFighters;

    private void Start()
    {
        manager.OnShopOptionsCreated += SetupShopOptions;

        options = new ShopOptionUI[6];

        ShopOptionUI[] outfits = outfitRect.transform.GetComponentsInChildren<ShopOptionUI>();
        ShopOptionUI[] fighters = fighterRect.transform.GetComponentsInChildren<ShopOptionUI>();

        Array.Copy(outfits, options, outfits.Length);
        Array.Copy(fighters, 0, options, outfits.Length, fighters.Length);

        Debug.Log(options.Length);
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

            if (shwowingFighters)
            {
                refreshFighters -= 40;
                refresh.text = $"{refreshFighters} SP";
            }
            else
            {
                refreshOutfits -= 40;
                refresh.text = $"{refreshOutfits} SP";
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
            fighterRect.localPosition = new Vector3(outfitRect.sizeDelta.x + 20f, fighterRect.localPosition.y, fighterRect.localPosition.z);
            LeanTween.moveLocalX(outfitRect.gameObject, -outfitRect.sizeDelta.x - 20f, 0.3f);
            LeanTween.moveLocalX(fighterRect.gameObject, 0f, 0.3f);

            refresh.text = $"{refreshFighters} SP";
            shopTitle.StringReference.SetReference("StringTable", "fighters");
        }
        else
        {
            outfitRect.localPosition = new Vector3(outfitRect.sizeDelta.x + 20f, outfitRect.localPosition.y, outfitRect.localPosition.z);
            LeanTween.moveLocalX(outfitRect.gameObject, 0f, 0.3f);
            LeanTween.moveLocalX(fighterRect.gameObject, -outfitRect.sizeDelta.x - 20f, 0.3f);

            refresh.text = $"{refreshOutfits} SP";
            shopTitle.StringReference.SetReference("StringTable", "outfits");
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
            fighterRect.localPosition = new Vector3(-outfitRect.sizeDelta.x - 20f, fighterRect.localPosition.y, fighterRect.localPosition.z);
            LeanTween.moveLocalX(outfitRect.gameObject, outfitRect.sizeDelta.x + 20f, 0.3f);
            LeanTween.moveLocalX(fighterRect.gameObject, 0f, 0.3f);

            refresh.text = $"{refreshFighters} SP";
            shopTitle.StringReference.SetReference("StringTable", "fighters");
        }
        else
        {
            outfitRect.localPosition = new Vector3(-outfitRect.sizeDelta.x - 20f, outfitRect.localPosition.y, outfitRect.localPosition.z);
            LeanTween.moveLocalX(outfitRect.gameObject, 0f, 0.3f);
            LeanTween.moveLocalX(fighterRect.gameObject, outfitRect.sizeDelta.x + 20f, 0.3f);

            refresh.text = $"{refreshOutfits} SP";
            shopTitle.StringReference.SetReference("StringTable", "outfits");
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
    }
}
