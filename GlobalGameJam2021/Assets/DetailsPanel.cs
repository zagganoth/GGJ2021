using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailsPanel : MonoBehaviour
{
    [SerializeField] Text panelDetails;
    HintSystem hintSystem;

    // Start is called before the first frame update
    void Start()
    {
        hintSystem = FindObjectOfType<HintSystem>();
    }

    public void UpdatePanelText(int colorIndex, int vehicleIndex)
    {
        string[] colors = hintSystem.GetColorNames();
        string[] vehicles = hintSystem.GetVehicleNames();
        panelDetails.text = "You selected a " + colors[colorIndex] + " " + vehicles[vehicleIndex];
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
