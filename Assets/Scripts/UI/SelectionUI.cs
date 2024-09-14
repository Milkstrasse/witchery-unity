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
        bool isReady = manager.SetReady(index);
        
        if (GlobalManager.singleton.maxPlayers < 2)
        {
            playerTop.ToggleUI(isReady);
        }

        playerBottom.ToggleUI(!isReady);
    }

    public (bool, bool) EditTeam(int fighterID)
    {
        return manager.EditTeam(fighterID);
    }

    private void StartFight()
    {
        playerTop.ToggleUI(false);
        playerBottom.ToggleUI(false);
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