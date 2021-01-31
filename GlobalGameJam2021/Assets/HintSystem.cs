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
    public string[] horzStreetNames = {"1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", "11th", "12th", "13th"};
    public string[] vertStreetNames = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M"};
    public GameObject streetLabel;

    List<GameObject> horzStreets = new List<GameObject>();
    List<GameObject> vertStreets = new List<GameObject>();

    private void Start() {
        mapInstance = MapGenerator.instance;
        CreateStreetLabels();
    }

    void CreateStreetLabels(){
        int spacing = mapInstance.gridSize;
        int currentPos = 0;
        while(currentPos < mapInstance.map_height){
            var lbl = Instantiate(streetLabel, new Vector3(mapInstance.map_width/2,0.15f,currentPos),Quaternion.identity) as GameObject;
            lbl.transform.eulerAngles = new Vector3(90, 0, 0);
            horzStreets.Add(lbl);
            currentPos += spacing;
        }

        currentPos = 0;
        while(currentPos < mapInstance.map_width){
            var lbl = Instantiate(streetLabel, new Vector3(currentPos,0.15f,mapInstance.map_height/2),Quaternion.identity) as GameObject;
            lbl.transform.eulerAngles = new Vector3(90, -90, 0);
            vertStreets.Add(lbl);
            currentPos += spacing;
        }

        for(int i = 0; i < horzStreets.Count; i++){
            horzStreets[i].GetComponentInChildren<TMP_Text>().text = horzStreetNames[i];
        }
        for(int i = 0; i < vertStreets.Count; i++){
            vertStreets[i].GetComponentInChildren<TMP_Text>().text = vertStreetNames[i];
        }
    }

    public string[] GetColorNames()
    {
        return colorNames;
    }
    public string[] GetVehicleNames()
    {
        return vehicleNames;
    }

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
        int x = Mathf.FloorToInt(pos.x/3);
        int y = Mathf.FloorToInt(pos.z/3);
        return "The suspect was last seen at the crossing of " + horzStreetNames[y] + " and " + vertStreetNames[x];
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
        if(vehicleNames[currentCriminal.vehicleIndex].Equals("person")){
            return "The suspect was last seen in " + colorNames[colorIndex] + " coloured clothing";
        }else{
            return "The suspect was last seen in a " + colorNames[colorIndex] + " coloured vehicle";
        }
        
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
        
        if(vehicleNames[vehicleIndex].Equals("person")){
            return "The suspect was last seen walking outside a vehicle";
        }else{
            return "The suspect was last seen in a " + vehicleNames[vehicleIndex];
        }
    }

    private ThiefAI GetCurrentCriminal()
    {
        return currentCriminal = GetComponent<GameSession>().GetCurrentCriminal();
    }


}
