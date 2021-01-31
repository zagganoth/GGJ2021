using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    HintSystem hintSystem;
    [SerializeField] Canvas detailsPanel;
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
    public ThiefAI currentCriminal;
    public ThiefAI currentAccusation;
    [SerializeField]
    float reportCooldown;
    bool reportCooldownActive;

    void Start()
    {
        reportCooldownActive = false;
        detailsPanel.enabled = false;
        BeginTimer();
        hintSystem = FindObjectOfType<HintSystem>();
    }

    void Update()
    {
        CheckTimeSinceLastHint();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            if (Physics.Raycast(ray, out hit)){
                GameObject other = hit.collider.gameObject;
                if(other.GetComponent<ThiefAI>()){
                    currentAccusation = other.GetComponent<ThiefAI>();
                    detailsPanel.enabled = true;
                    detailsPanel.GetComponent<DetailsPanel>().UpdatePanelText(currentAccusation.colorIndex, currentAccusation.vehicleIndex);
                }
            }
        }
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

    public void Report()
    {
        if (!reportCooldownActive) {
            if (currentAccusation == currentCriminal)
            {
                Debug.Log("You did it!");
            }
            else
            {
                StartCoroutine(ReportCooldown());
            }
        }
        else
        {
            Debug.Log("Cannot dispatch police. Please wait a few seconds");
        }
    }
    private IEnumerator ReportCooldown()
    {
        reportCooldownActive = true;
        yield return new WaitForSeconds(reportCooldown);
        reportCooldownActive = false;

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

    public float GetDuaration()
    {
        return gameLength;
    }

}
