using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShirtColour
{
    Red,
    Blue,
    Yellow,
}

public class Criminal : MonoBehaviour
{
    System.Random random = new System.Random();
    
    List<string> shirtColours = new List<string>();
    string criminalShirtColour;
    [SerializeField] Material red;
    [SerializeField] Material blue;
    [SerializeField] Material yellow;

    Vector3 currentLocation = new Vector3();

    void Start()
    {
        criminalShirtColour = SetShirtColour();
        SetMaterial();
    }


    private void SetMaterial()
    {
        if (criminalShirtColour == "Red")
        {
            GetComponent<Renderer>().material = red;
        }
        else if (criminalShirtColour == "Blue")
        {
            GetComponent<Renderer>().material = blue;
        }
        else if (criminalShirtColour == "Yellow")
        {
            GetComponent<Renderer>().material = yellow;
        }
    }

    public string GetShirtColour()
    {
        return criminalShirtColour;
    }

    public string SetShirtColour()
    {
        shirtColours.Add("Red");
        shirtColours.Add("Blue");
        shirtColours.Add("Yellow");

        int index = random.Next(0, shirtColours.Count);
        return shirtColours[index];
    }

    public List<string> GetAllShirtColours()
    {
        return shirtColours;
    }

    public Vector3 GetCurrentLocation()
    {
        return currentLocation = transform.position;
    }

}
