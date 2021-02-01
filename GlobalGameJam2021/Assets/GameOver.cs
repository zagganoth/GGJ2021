using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] GameObject gameOverSound;
    public GameObject music;


    void Start()
    {
        gameOverCanvas.enabled = false;
    }

    public void GameOverUI()
    {
        gameOverCanvas.enabled = true;
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
