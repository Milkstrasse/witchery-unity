using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public void ReturnToMenu()
    {
        GlobalManager.singleton.LoadScene("MenuScene");
    }
}
