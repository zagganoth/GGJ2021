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
    HashSet<int> visitedLocations;
    bool visitedEverything;
    bool pathActive;
    // Start is called before the first frame update
    void Start()
    {
        curStraightDest = transform.position;
        visitedLocations = new HashSet<int>();
        changeState(startState);
        visitedEverything = false;
        pathActive = false;
    }
    public void changeState(BaseState newState)
    {
        newState.Enter(this);
        StartCoroutine(newState.Perform());
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
    public void visitLocation(int index)
    {
        visitedLocations.Add(index);
    }
    public HashSet<int> getVisitedLocations() {
        return visitedLocations;
    }
    public void setVisitedEverything()
    {
        visitedEverything = true;
    }
    public bool hasVisitedEverything()
    {
        return visitedEverything;
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
        Debug.Log("Then moving");
        changeState(startState);
    }
    // Update is called once per frame
    void Update()
    {
        if (!visitedEverything &&positions != null)
        {
            if (positions.Count > 0)
            {
                if (Vector3.Distance(curStraightDest, transform.position) < 0.3f)
                {
                    var tempDest = positions.Pop();
                    curStraightDest = new Vector3(tempDest.x, 0.1f, tempDest.y);
                    if(positions.Count == 0)
                    {
                        pathActive = false;
                    }
                }
                transform.position = Vector3.MoveTowards(transform.position, curStraightDest, speed * Time.deltaTime);
            }
            else if(!pathActive)
            {
                StartCoroutine(waitThenMove());
                pathActive = true;
            }
        }
        

    }
}
