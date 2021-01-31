using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    HintSystem hintSystem;

    //TIMER CODE
    private TimeSpan gameTime;
    private TimeSpan gameDuration;
    private bool gameStarted = false;
    private float elapsedTime;
    [SerializeField] float gameLength = 15f;

    //HINT CODE
    [SerializeField] float hintInterval = 10f;
    float timeSinceLastHint = 0;

    //SPAWN CRIMINAL CODE\
    [SerializeField] Criminal criminal;
    public ThiefAI currentCriminal;
    string criminalShirtColour;

    void Start()
    {
        BeginTimer();
        hintSystem = FindObjectOfType<HintSystem>();
    }

    void Update()
    {
        CheckTimeSinceLastHint();
    }

    private void CheckTimeSinceLastHint()
    {
        timeSinceLastHint += Time.deltaTime;
        if (timeSinceLastHint >= hintInterval)
        {
            hintSystem.SendHint();
            timeSinceLastHint = 0f;
        }
        else { return; }
    }

    public ThiefAI GetCurrentCriminal()
    {
        return currentCriminal;
    }


    private IEnumerator StartGameTimer()
    {
        while (gameStarted)
        {
            gameDuration = TimeSpan.FromSeconds(gameLength); 
            if (gameTime < gameDuration)
            {
                elapsedTime += Time.deltaTime;
                gameTime = TimeSpan.FromSeconds(elapsedTime);
            }
            else
            {
                gameStarted = false;
                GameOver();
            }

            yield return null;
        }
    }

    private void BeginTimer()
    {
        gameStarted = true;
        elapsedTime = 0f;
        StartCoroutine("StartGameTimer");
    }

    private void GameOver()
    {
        print("the game is over");
    }

}
