using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameWin : MonoBehaviour
{
    [SerializeField] Canvas gameWinCanvas;

    void Start()
    {
        gameWinCanvas.enabled = false;
    }

    public void GameWinUI()
    {
        gameWinCanvas.enabled = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }



}
