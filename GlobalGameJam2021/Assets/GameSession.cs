using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    HintSystem hintSystem;
    [SerializeField] Canvas detailsPanel;
    [SerializeField] Canvas cooldownPanel;

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
        Time.timeScale = 1;
        reportCooldownActive = false;
        detailsPanel.enabled = false;
        cooldownPanel.enabled = false;
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
            hintSystem.SetHints();
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
                Time.timeScale = 0;
                GetComponent<GameWin>().GameWinUI();
            }
            else
            {
                cooldownPanel.enabled = true;
                cooldownPanel.GetComponent<MessageCanvas>().DisplayNotHereMsg();
                cooldownPanel.GetComponent<MessageCanvas>().DisplayCooldown(reportCooldown);
                StartCoroutine(ReportCooldown());
            }
        }
        else
        {
            cooldownPanel.enabled = true;
        }
    }
    private IEnumerator ReportCooldown()
    {
        reportCooldownActive = true;
        yield return new WaitForSeconds(reportCooldown);
        reportCooldownActive = false;
        cooldownPanel.enabled = false;
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
        Time.timeScale = 0;
        GetComponent<GameOver>().GameOverUI();
    }

    public float GetDuaration()
    {
        return gameLength;
    }

}
