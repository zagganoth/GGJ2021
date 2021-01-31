using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefAI : MonoBehaviour
{
    MapGenerator mapStance;
    HashSet<Vector2Int> visitedLocations;
    Stack<Vector2Int> currentPath;
    Vector2Int destination;
    public Vector3 curStraightDest;
    bool visitedEverything;
    bool pathActive;

    [Header("Agent settings")]
    [SerializeField]
    bool isThief = false;
    public int colorIndex = 0;
    public int vehicleIndex = 0;
    
    [SerializeField]
    BaseState startState;
    [SerializeField]
    float speed;
    [SerializeField]
    bool loopAfterVisitingAll;
    [SerializeField]
    bool onlyVisitDestinationsOnce;

    void Start()
    {
        curStraightDest = transform.position;
        visitedLocations = new HashSet<Vector2Int>();
        changeState(startState);
        visitedEverything = false;
        pathActive = false;
        mapStance = MapGenerator.instance;
    }
    public void changeState(BaseState newState)
    {
        var clone = Instantiate(newState);
        clone.Enter(this);
        StartCoroutine(clone.Perform());
    }

    public void setDestination(Vector2Int dest)
    {
        pathActive = true;
        destination = dest; 
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
        return visitedEverything;
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
            //Debug.Log("No path found to " + destination);
            return;
        }
        var tempDest = currentPath.Pop();
        curStraightDest = new Vector3(tempDest.x, 0.0f, tempDest.y);
        pathActive = true;
        //Debug.Log("Moving towards: " + curStraightDest);
    }
    private IEnumerator waitThenMove()
    {
        yield return new WaitForSeconds(3f);
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
                Vector3 newPos = Vector3.MoveTowards(transform.position, curStraightDest, speed * Time.deltaTime);
                //Debug.Log("Position is " + transform.position);
                //Debug.Log("Moving to " + curStraightDest);
                if(pathActive){
                    if (curStraightDest.x > transform.position.x) //right
                    {
                        //Debug.Log("Rotating right");
                        //transform.rotation = new Quaternion(transform.rotation.x, 270, transform.rotation.z, transform.rotation.w);
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 270 , transform.eulerAngles.z);
                        //Debug.Log(transform.rotation.y);
                    }
                    else if (curStraightDest.x < transform.position.x) // left
                    {
                        //Debug.Log("Rotating left");
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90, transform.eulerAngles.z);
                        //transform.rotation = new Quaternion(transform.rotation.x, 90, transform.rotation.z, transform.rotation.w);
                        //Debug.Log(transform.rotation.y);
                    }
                    else if (curStraightDest.z < transform.position.z) // down 
                    {
                        //Debug.Log("Rotating down");
                        //transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x,  0, transform.eulerAngles.z);
                    }
                    else if(curStraightDest.z > transform.position.z) // up 
                    {
                        //Debug.Log("Rotating up");
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
                        //transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
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
