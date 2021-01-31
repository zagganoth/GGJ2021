using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    [SerializeField] Canvas gameOverCanvas;

    void Start()
    {
        gameOverCanvas.enabled = false;
    }

    public void GameOverUI()
    {
        gameOverCanvas.enabled = true;
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
