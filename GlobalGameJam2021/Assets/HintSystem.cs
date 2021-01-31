using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintSystem : MonoBehaviour
{
    System.Random random = new System.Random();
    MapGenerator mapInstance;
    int randomHint;
    ThiefAI currentCriminal;

    int hintSlot1;
    int hintSlot2;
    [SerializeField] TMP_Text hintNumberOne;
    [SerializeField] TMP_Text hintNumberTwo;

    List<string> trueHints = new List<string>();
    List<string> falseHints = new List<string>();
    string currentTrueHint;
    string currentFalseHint;

    public string[] colorNames;
    public string[] vehicleNames;
    public string[] horzStreetNames;
    public string[] vertStreetNames;

    private void Start() {
        mapInstance = MapGenerator.instance;
    }

    public void SendHint()
    {
        hintSlot1 = SetHintSlotOne();
        if (hintSlot1 == 0)
        {
            hintNumberOne.text = GetTrueHint();
            hintNumberTwo.text = GetTrueHint();//GetFalseHint();
        }
        else if (hintSlot1 == 1)
        {
            hintNumberOne.text = GetFalseHint();
            hintNumberTwo.text = GetTrueHint();//GetTrueHint();
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
        //print(currentTrueHint);
        return currentTrueHint;
    }

    public string GetFalseHint()
    {
        BuildFalseHintsList();
        randomHint = random.Next(0, falseHints.Count);
        currentFalseHint = falseHints[randomHint];
        //print(currentFalseHint);
        return currentFalseHint;
    }

    private void BuildFalseHintsList()
    {
        falseHints.Add(GetColorHint(false));
        falseHints.Add(GetVehicleHint(false));
        falseHints.Add(GetSightingHint(false));
    }

    private void BuildTrueHintsList()
    {
        trueHints.Add(GetColorHint(true));
        trueHints.Add(GetVehicleHint(true));
        trueHints.Add(GetSightingHint(true));
    }

    private string GetSightingHint(bool truth)
    {
        GetCurrentCriminal();
        Vector3 pos;
        if(truth){
            pos = currentCriminal.transform.position;
        }else{
            pos = new Vector3 (random.Next(0, mapInstance.map_width), 0, random.Next(0, mapInstance.map_height));
        }
        pos = pos/mapInstance.gridSize;
        return "The suspect was last seen at the crossing of " + horzStreetNames[(int)pos.z] + " and " + vertStreetNames[(int)pos.x];
    }

    string GetColorHint(bool truth){
        GetCurrentCriminal();
        int colorIndex = 0;
        if(truth){
            colorIndex = currentCriminal.colorIndex;
        }else{
            while(colorIndex == currentCriminal.colorIndex){
                colorIndex = random.Next(0, colorNames.Length-1);
            }
        }
        return "The suspect was last seen in a " + colorNames[colorIndex] + " coloured vehicle";
    }

    string GetVehicleHint(bool truth){
        GetCurrentCriminal();
        int vehicleIndex = 0;
        if(truth){
            vehicleIndex = currentCriminal.vehicleIndex;
        }else{
            while(vehicleIndex == currentCriminal.vehicleIndex){
                vehicleIndex = random.Next(0, vehicleNames.Length-1);
            }
        }
        return "The suspect was last seen in a " + vehicleNames[vehicleIndex];
    }

    private ThiefAI GetCurrentCriminal()
    {
        return currentCriminal = GetComponent<GameSession>().GetCurrentCriminal();
    }


}
