using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Date : MonoBehaviour
{
    [SerializeField] GameSession gameSession;
    float timeRemaining;
    float oneDay;

[SerializeField] TMP_Text dateBox;
    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = gameSession.GetDuaration();
        oneDay = gameSession.GetDuaration() / 5;
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;
        print(timeRemaining);
        if (timeRemaining > (oneDay * 4))
        {
            dateBox.text = "MON JAN 01";
        }
        else if (timeRemaining < (oneDay * 4) && timeRemaining > (oneDay * 3))
        {
            dateBox.text = "TUE JAN 02";
        }
        else if (timeRemaining < (oneDay * 3) && timeRemaining > (oneDay * 2))
        {
            dateBox.text = "WED JAN 03";
        }
        else if (timeRemaining < (oneDay * 2) && timeRemaining > (oneDay * 1))
        {
            dateBox.text = "THU JAN 04";
        }
        else if (timeRemaining < (oneDay * 1))
        {
            dateBox.text = "FRI JAN 05";
        }

    }
}
