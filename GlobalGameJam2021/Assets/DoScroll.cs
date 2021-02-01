using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoScroll : MonoBehaviour
{
    public float scrollSpeed;
    public RectTransform rect;
    public Vector3 origPosition;
    public TMP_Text childText;
    private MapGenerator mapStance;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        //DoScroll[] others = FindObjectsOfType(typeof(DoScroll)) as DoScroll[];
        origPosition = transform.position;
        /*foreach (var other in others)
        {
            if(other != this)
            {
                origPosition = other.transform.position;
            }
        }*/

        childText = GetComponentInChildren<TMP_Text>();
        mapStance = MapGenerator.instance;
    }

    // Update is called once per frame
    void Update()
    {
        childText.text = mapStance.bannerText;
        transform.Translate(Vector3.left * Time.deltaTime * scrollSpeed);
        Debug.Log(transform.position);
        if(transform.position.x < 0)
        {
            transform.position = origPosition + new Vector3(1920,0,0);
        }
    }
}
