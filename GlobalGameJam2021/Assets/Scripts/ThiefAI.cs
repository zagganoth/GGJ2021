﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefAI : MonoBehaviour
{
    MapGenerator mapStance;
    HashSet<Vector2Int> visitedLocations;
    Stack<Vector2Int> currentPath;
    Vector2Int destination;
    GameObject destinationBuilding;
    public Vector3 curStraightDest;
    [SerializeField]
    bool visitedEverything;
    [SerializeField]
    bool pathActive;

    [Header("Agent settings")]
    public bool isThief = false;
    public int colorIndex = 0;
    public int vehicleIndex = 0;
    public BaseState currentState;
    
    [SerializeField]
    BaseState startState;
    public float speed;
    [SerializeField]
    bool loopAfterVisitingAll;
    [SerializeField]
    bool onlyVisitDestinationsOnce;
    [SerializeField]
    BaseState state;
    public bool justStolen;
    void Start()
    {
        curStraightDest = transform.position;
        visitedLocations = new HashSet<Vector2Int>();
        changeState(startState);
        visitedEverything = false;
        pathActive = false;
        mapStance = MapGenerator.instance;
        justStolen = false;
        if(isThief)
        {
            onlyVisitDestinationsOnce = true;
            loopAfterVisitingAll = true;
        }
    }
    public void changeState(BaseState newState)
    {
        var clone = Instantiate(newState);
        state = clone;
        clone.Enter(this);
        StartCoroutine(clone.Perform());
        currentState = newState;
    }

    public void setDestination(Vector2Int dest, GameObject building)
    {
        pathActive = true;
        destination = dest;
        destinationBuilding = building;
    }
    public Vector2Int getDestination()
    {
        return destination;
    }
    public void visitLocation(Vector2Int location)
    {
        if (onlyVisitDestinationsOnce)
        {
            visitedLocations.Add(location);
        }
    }
    public HashSet<Vector2Int> getVisitedLocations() {
        return visitedLocations;
    }
    public void setVisitedEverything()
    {
        visitedEverything = true;
        if(loopAfterVisitingAll)
        {
            StartCoroutine(clearVisited());
        }
    }
    public bool hasVisitedEverything()
    {
        return visitedEverything && onlyVisitDestinationsOnce;
    }
    IEnumerator clearVisited()
    {
        yield return new WaitForSeconds(5f);
        visitedEverything = false;
        visitedLocations = new HashSet<Vector2Int>();
        changeState(startState);
    }

    public void setPathPositions(Stack<Vector2Int> pos)
    {
        currentPath = pos;
        if(currentPath.Count == 0)
        {
            pathActive = false;
            return;
        }
        var tempDest = currentPath.Pop();
        curStraightDest = new Vector3(tempDest.x, 0.1f, tempDest.y);
        //Debug.Log("Moving towards: " + curStraightDest);
    }
    private IEnumerator waitThenMove()
    {

        state = startState;
        Renderer childRenderer;
        if (isThief && !destinationBuilding.CompareTag("Building"))
        {
            childRenderer = destinationBuilding.transform.GetChild(0).GetComponent<Renderer>();
            Material cur = childRenderer.material;
            childRenderer.material = mapStance.red;
            Instantiate(mapStance.newsFlare, Vector3.zero, Quaternion.identity);
            mapStance.updateBanner(destinationBuilding.tag, true);
            yield return new WaitForSeconds(0.25f);
            childRenderer.material = cur;
            yield return new WaitForSeconds(1f);
            childRenderer.material = mapStance.red;
            yield return new WaitForSeconds(0.25f);
            childRenderer.material = cur;
            yield return new WaitForSeconds(1.75f);
            mapStance.updateBanner(destinationBuilding.tag, false);
            mapStance.addRobbedAmount();
            justStolen = true;
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(0.1f,10f));
            justStolen = false;
        }
        //Debug.Log("Then moving");
        changeState(startState);
    }
    // Update is called once per frame
    void Update()
    {
        if (!visitedEverything && currentPath != null)
        {
            if (currentPath.Count > 0)
            {
                if (Vector3.Distance(curStraightDest, transform.position) == 0)
                {
                    var tempDest = currentPath.Pop();
                    curStraightDest = new Vector3(tempDest.x, 0.1f, tempDest.y);
                    if(currentPath.Count == 0)
                    {
                        pathActive = false;
                    }
                }
                Vector3 direction = (curStraightDest - transform.position).normalized;
                float speedModifier = 1;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, 1f))
                {
                    speedModifier = 0.5f;
                }
                Vector3 newPos = Vector3.MoveTowards(transform.position, curStraightDest, speed * Time.deltaTime * speedModifier);

                
                if(pathActive){
                    if (curStraightDest.x > transform.position.x) //right
                    {
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 270 , transform.eulerAngles.z);
                    }
                    else if (curStraightDest.x < transform.position.x) // left
                    {
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90, transform.eulerAngles.z);
                    }
                    else if (curStraightDest.z < transform.position.z) // down 
                    {
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x,  0, transform.eulerAngles.z);
                    }
                    else if(curStraightDest.z > transform.position.z) // up 
                    {
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
                    }
                }


                transform.position = newPos;
            }
            else if(!pathActive)
            {
                StartCoroutine(waitThenMove());
                pathActive = true;
            }
        }
        

    }
}
