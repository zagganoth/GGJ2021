using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameWin : MonoBehaviour
{
    [SerializeField] Canvas gameWinCanvas;
    [SerializeField] GameObject gameOverSound;
    public GameObject music;

    void Start()
    {
        gameWinCanvas.enabled = false;
    }

    public void GameWinUI()
    {
        gameWinCanvas.enabled = true;
        Instantiate(gameOverSound,Vector3.zero, Quaternion.identity);
        music.SetActive(false);
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
