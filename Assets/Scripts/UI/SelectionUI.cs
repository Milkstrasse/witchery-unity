using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionUI : MonoBehaviour
{
    [SerializeField] private SelectionManager manager;
    
    [SerializeField] private PlayerSelectionUI playerTop;
    [SerializeField] private PlayerSelectionUI playerBottom;

    private PlayerObject[] players;

    private void Start()
    {
        manager.OnTimerChanged += playerBottom.SetTimer;
        manager.OnPlayersReady += StartFight;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new PlayerObject[gameObjects.Length];

        if (players.Length > 0)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                players[i] = gameObjects[i].GetComponent<PlayerObject>();
            }

            if (GlobalManager.singleton.mode != GameMode.Online || NetworkClient.activeHost)
            {
                playerTop.SetName(players[1].playerName);

                for (int j = 0; j < players[0].fighterIDs.Length; j++)
                {
                    playerBottom.SelectCard(players[0].fighterIDs[j], true);
                }
            }
            else
            {
                playerTop.SetName(players[0].playerName);

                for (int j = 0; j < players[1].fighterIDs.Length; j++)
                {
                    playerBottom.SelectCard(players[1].fighterIDs[j], true);
                }
            }
        }
    }

    public void SetReady(int index)
    {
        bool isReady = manager.SetReady(index);

        if (isReady)
        {
            AudioManager.singleton.PlayPositiveSound();
        }
        else
        {
            AudioManager.singleton.PlayNegativeSound();
        }
        
        if (GlobalManager.singleton.mode == GameMode.Offline || GlobalManager.singleton.mode == GameMode.Testing)
        {
            playerTop.ToggleUI(isReady);

            if (players.Length > 1 && isReady)
            {
                for (int j = 0; j < players[1].fighterIDs.Length; j++)
                {
                    playerTop.SelectCard(players[1].fighterIDs[j], true);
                }
            }
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
        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }

        playerTop.ToggleUI(false, true);
        playerBottom.ToggleUI(false, true);

        GlobalManager.singleton.LoadScene("FightScene", LoadSceneMode.Additive);
    }

    public void StopSelection()
    {
        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }
        
        manager.ReturnToMenu();
    }

    private void OnDestroy()
    {
        manager.OnTimerChanged -= playerBottom.SetTimer;
        manager.OnPlayersReady -= StartFight;
    }
}