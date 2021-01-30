using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefAI : MonoBehaviour
{
    [SerializeField]
    BaseState startState;
    private Vector2Int destination;
    // Start is called before the first frame update
    void Start()
    {
        changeState(startState);
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
