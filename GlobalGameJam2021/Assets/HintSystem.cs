using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintSystem : MonoBehaviour
{
    System.Random random = new System.Random();
    int randomHint;
    Criminal currentCriminal;

    int hintSlot1;
    int hintSlot2;
    [SerializeField] TMP_Text hintNumberOne;
    [SerializeField] TMP_Text hintNumberTwo;

    List<string> trueHints = new List<string>();
    List<string> falseHints = new List<string>();
    string currentTrueHint;
    string currentFalseHint;

    public void SendHint()
    {
        hintSlot1 = SetHintSlotOne();
        if (hintSlot1 == 0)
        {
            hintNumberOne.text = GetTrueHint();
            hintNumberTwo.text = GetFalseHint();
        }
        else if (hintSlot1 == 1)
        {
            hintNumberOne.text = GetFalseHint();
            hintNumberTwo.text = GetTrueHint();
        }
    }

    private int SetHintSlotOne()
    {
        int slot = random.Next(0, 1);
        return slot;
    
    }

    public string GetTrueHint()
    {
        BuildTrueHintsList();
        randomHint = random.Next(0, trueHints.Count);
        currentTrueHint = trueHints[randomHint];
        print(currentTrueHint);
        return currentTrueHint;
    }

    public string GetFalseHint()
    {
        BuildFalseHintsList();
        randomHint = random.Next(0, falseHints.Count);
        currentFalseHint = falseHints[randomHint];
        print(currentFalseHint);
        return currentFalseHint;
    }

    private void BuildFalseHintsList()
    {
        falseHints.Add(FalseShirtColour());
    }

    private void BuildTrueHintsList()
    {
        trueHints.Add(TrueShirtColour());
        trueHints.Add(TrueSighting());
    }

    private string TrueShirtColour()
    {
        GetCurrentCriminal();
        return "The suspect was last seen wearing a " + currentCriminal.GetShirtColour() + " coloured shirt";
    }

    private string TrueSighting()
    {
        GetCurrentCriminal();
        return "The suspect was last seen at " + currentCriminal.GetCurrentLocation();
    }

    private string FalseShirtColour()
    {
        GetCurrentCriminal();
        string criminalShirtColour = currentCriminal.GetShirtColour();
        List<string> shirtColours = currentCriminal.GetAllShirtColours();
        randomHint = random.Next(0, shirtColours.Count);
        if (shirtColours[randomHint] != criminalShirtColour)
        {
            return "The suspect was last seen wearing a " + shirtColours[randomHint] + " coloured shirt";
        }
        else
        {
            return FalseShirtColour();
        }
    }


    private Criminal GetCurrentCriminal()
    {
        return currentCriminal = GetComponent<GameSession>().GetCurrentCriminal();
    }


}
