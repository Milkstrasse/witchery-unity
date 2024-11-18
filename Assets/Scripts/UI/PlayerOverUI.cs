using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PlayerOverUI : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent title;
    [SerializeField] private LocalizeStringEvent description;
    [SerializeField] private Image portrait;
    [SerializeField] private RawImage background;
    [SerializeField] private Image triangle;

    [SerializeField] private Material victory;
    [SerializeField] private Material defeat;

    public void UpdateUI(PlayerObject player)
    {
        title.StringReference.SetReference("StringTable", player.hasWon ? "victory" : "defeat");

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalData.fighters[player.icon].name + "-standard");

        background.material = player.hasWon ? victory : defeat;
        triangle.material = player.hasWon ? victory : defeat;
    }
}
