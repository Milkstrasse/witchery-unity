using UnityEngine;

public class SelectionUI : MonoBehaviour
{
    [SerializeField] private SelectionManager manager;
    
    [SerializeField] private PlayerSelectionUI playerTop;
    [SerializeField] private PlayerSelectionUI playerBottom;

    private void Start()
    {
        manager.OnTimerChanged += playerBottom.SetTimer;
        manager.OnPlayersReady += StartFight;
    }

    public void SetReady(int index)
    {
        AudioManager.singleton.PlayStandardSound();
        
        bool isReady = manager.SetReady(index);
        
        if (GlobalManager.singleton.mode == GameMode.Offline)
        {
            playerTop.ToggleUI(isReady);
        }

        playerBottom.ToggleUI(!isReady);
    }

    public SelectionResult EditTeam(SelectedFighter fighter)
    {
        return manager.EditTeam(fighter);
    }

    public void EditTeam(int fighter, int outfit)
    {
        manager.EditTeam(fighter, outfit);
    }

    private void StartFight()
    {
        playerTop.ToggleUI(false, true);
        playerBottom.ToggleUI(false, true);
    }

    public void StopSelection()
    {
        manager.ReturnToMenu();
    }

    private void OnDestroy()
    {
        manager.OnTimerChanged -= playerBottom.SetTimer;
        manager.OnPlayersReady -= StartFight;
    }
}