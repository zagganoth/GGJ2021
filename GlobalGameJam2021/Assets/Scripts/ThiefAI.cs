using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefAI : MonoBehaviour
{
    [SerializeField]
    BaseState startState;
    private Vector2Int destination;
    Stack<Vector2Int> positions;
    Vector3 curStraightDest;
    [SerializeField]
    float speed;
    HashSet<Vector2Int> visitedLocations;
    bool visitedEverything;
    bool pathActive;
    MapGenerator mapStance;
    [SerializeField]
    bool loopAfterVisitingAll;
    [SerializeField]
    bool noVisit;
    // Start is called before the first frame update
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
        destination = dest; 
    }
    public Vector2Int getDestination()
    {
        return destination;
    }
    public Vector3 getPos()
    {
        return transform.position;
    }
    public void visitLocation(Vector2Int index)
    {
        if (!noVisit)
        {

            visitedLocations.Add(index);
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
    private Vector3 getAdjacentRoad(bool[,] roads, Vector3 position)
    {
        int nodeX = Mathf.FloorToInt(position.x);
        int nodeY = Mathf.FloorToInt(position.y);

        if (nodeX - 1 >= 0 && roads[nodeX - 1, nodeY])
        {
            return new Vector3(nodeX - 1, transform.position.y, nodeY);
        }
        else if (nodeX + 1 < roads.GetLength(0) && roads[nodeX + 1, nodeY])
        {
            return new Vector3(nodeX + 1, transform.position.y, nodeY);
        }
        else if (nodeY - 1 >= 0 &&  roads[nodeX, nodeY - 1])
        {
            return new Vector3(nodeX, transform.position.y, nodeY - 1);
        }
        else if(nodeY + 1 < roads.GetLength(1))
        {
            return new Vector3(nodeX, transform.position.y, nodeY + 1);
        }
        return position;
    }
    public void setPositions(Stack<Vector2Int> pos)
    {
        positions = pos;
        if(positions.Count == 0)
        {
            //Debug.Log("No path found to " + destination);
            return;
        }
        var tempDest = positions.Pop();
        curStraightDest = new Vector3(tempDest.x, 0.1f, tempDest.y);
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
        if (!visitedEverything &&positions != null)
        {
            if (positions.Count > 0)
            {
                if (Vector3.Distance(curStraightDest, transform.position) == 0)
                {
                    var tempDest = positions.Pop();
                    curStraightDest = new Vector3(tempDest.x, 0.1f, tempDest.y);
                    if(positions.Count == 0)
                    {
                        pathActive = false;
                    }
                }
                Vector3 newPos = Vector3.MoveTowards(transform.position, curStraightDest, speed * Time.deltaTime);
                //Debug.Log("Position is " + transform.position);
                //Debug.Log("Moving to " + curStraightDest);
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


                transform.position = newPos;
                /*int testX = Mathf.FloorToInt(transform.position.x);
                int testY = Mathf.FloorToInt(transform.position.z);
                if(!mapStance.roads[testX,testY])
                {
                    transform.position = getAdjacentRoad(mapStance.roads, transform.position);
                }*/
            }
            else if(!pathActive)
            {
                StartCoroutine(waitThenMove());
                pathActive = true;
            }
        }
        

    }
}
