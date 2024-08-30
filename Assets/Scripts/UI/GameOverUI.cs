using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameOverManager manager;
    [SerializeField] private TextMeshProUGUI winText;

    private void Awake()
    {
        manager.OnSetupComplete += SetupUI;
    }

    private void SetupUI(int player)
    {
        winText.text = $"Player {player} has won";
    }

    public void ReturnToMenu()
    {
        manager.ReturnToMenu();
    }

    private void OnDestroy()
    {
        manager.OnSetupComplete -= SetupUI;
    }
}
