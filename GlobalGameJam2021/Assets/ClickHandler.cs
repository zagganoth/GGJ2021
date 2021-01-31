using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    [SerializeField] Canvas detailsPanel;
    // Start is called before the first frame update
    void Start()
    {
        detailsPanel.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            detailsPanel.enabled = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);
            Debug.Log("This hit at " + hit.point );
            GameObject other = hit.collider.gameObject;
            if(other.GetComponent<ThiefAI>()){
                var thief = other.GetComponent<ThiefAI>();
                detailsPanel.enabled = true;
                detailsPanel.GetComponent<DetailsPanel>().UpdatePanelText(thief.colorIndex, thief.vehicleIndex);
                Debug.Log(thief.colorIndex);
            }
        }
    }
}
