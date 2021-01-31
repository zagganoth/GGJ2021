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
    GameObject child;
    Renderer renderer;
    Vector3 currentLocation = new Vector3();

    void Start()
    {
        child = transform.GetChild(0).gameObject;
        renderer = child.GetComponent<Renderer>();
        criminalShirtColour = SetShirtColour();
        SetMaterial();
    }


    private void SetMaterial()
    {
        if (criminalShirtColour == "Red")
        {
            renderer.material = red;
        }
        else if (criminalShirtColour == "Blue")
        {
            renderer.material = blue;
        }
        else if (criminalShirtColour == "Yellow")
        {
            renderer.material = yellow;
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
