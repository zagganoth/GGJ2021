using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/DeterminePath")]
public class DeterminePath : BaseState
{
    Dictionary<Vector2Int, Vector2Int> parentDict;
    public class HeapNode : IComparable
    {
        
        public Vector2Int position;
        Vector2Int dest;
        public int pathCost;
        float heuristic;
        public float f;
        public HeapNode(int depth, Vector2Int curPos, Vector2Int destination)
        {
            position = curPos;
            dest = destination;
            pathCost = depth;
            heuristic = getHeuristic();
            f = getF();
        }
        private float getHeuristic()
        {
            return Vector2Int.Distance(position, dest);
        }
        private float getF()
        {
            return heuristic + pathCost;
        }
        public int CompareTo(object obj)
        { 
            HeapNode other = obj as HeapNode;
            int ret = f.CompareTo(other.f);
            return ret;

        }
    }
    private Stack<Vector2Int> path;
    public MapGenerator mapStance;
    protected override void childEnter(ThiefAI cur)
    {
        mapStance = MapGenerator.instance;

    }
    private float heuristic(Vector2Int src, Vector2Int dest)
    {
        return Vector2Int.Distance(src, dest);
    }
    private bool isIntersection(bool[,] roads, Vector2Int position)
    {
        int nodeX = position.x;
        int nodeY = position.y;
        if(nodeX < 0 || nodeY < 0 || nodeX >= roads.GetLength(0) || nodeY >= roads.GetLength(1))
        {
            return false;
        }
        bool pass = false;
        pass |= (nodeX + 1 < roads.GetLength(0) && nodeY + 1 < roads.GetLength(1) && (roads[nodeX + 1, nodeY] && roads[nodeX, nodeY + 1]));  //right and up
        pass |= (nodeX + 1 < roads.GetLength(0) && nodeY - 1 >= 0 && (roads[nodeX + 1, nodeY] && roads[nodeX, nodeY - 1])); // right and down
        pass |= (nodeX - 1 >= 0 && nodeY + 1 < roads.GetLength(1) && (roads[nodeX - 1, nodeY] && roads[nodeX, nodeY + 1])); // left and up
        pass |= (nodeX - 1 >= 0 && nodeY -1 >= 0 && (roads[nodeX - 1, nodeY] && roads[nodeX, nodeY - 1]));  // left and down
        return pass;
    }
    private Vector2Int getAdjacentRoad(bool [,] roads, Vector2Int position)
    {
        int nodeX = position.x;
        int nodeY = position.y;
        if(roads[nodeX-1,nodeY])
        {
            return new Vector2Int(nodeX-1,nodeY);
        }
        else if(roads[nodeX+1,nodeY])
        {
            return new Vector2Int(nodeX+1,nodeY);
        }
        else if(roads[nodeX,nodeY-1])
        {
            return new Vector2Int(nodeX, nodeY - 1);
        }
        else
        {
            return new Vector2Int(nodeX, nodeY + 1);
        }
    }
    public override IEnumerator Perform()
    {
        yield return null;
        parentDict = new Dictionary<Vector2Int, Vector2Int>();
        Vector2Int dest = self.getDestination();
        Vector2Int origDest = dest;
        Vector3 pos = self.getPos();
        bool[,] roads = mapStance.roads;
        dest = getAdjacentRoad(roads, dest);
        Vector2Int convertedPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
        //Debug.Log("Current position of " + self + " is " + convertedPos);
        //Debug.Log("Destination is " + dest);
        //Tuple<Vector2Int, Vector2Int> node;
        PriorityQueue<HeapNode> nodes = new PriorityQueue<HeapNode>(1000);
        nodes.Insert(1, new HeapNode(0, convertedPos,dest));
        Vector2Int finalPos = dest;
        int index = 0;
        while(nodes.Count > 0)
        {
            //Debug.Log("On iteration " + index);
            var node = nodes.Pop();
            //Debug.Log(nodes);
            //Debug.Log("This line runs.");
            //Debug.Log("Exploring position: " + node.position);
            //Debug.Log("current f is " + node.f);
            int nodeX = node.position.x;
            int nodeY = node.position.y;
            if(node.position == dest)
            {
                finalPos = node.position;
                break;
            }
            Vector2Int left = new Vector2Int(nodeX - 1, nodeY);
            if (node.f > 8)
            {
                while (left.x >= 0 && !isIntersection(roads, left))
                {
                    left = new Vector2Int(left.x - 1, nodeY);
                }
            }
            Vector2Int right = new Vector2Int(nodeX + 1, nodeY);
            if (node.f > 8)
            {
                while (right.x < roads.GetLength(0) && !isIntersection(roads, right))
                {
                    right = new Vector2Int(right.x + 1, nodeY);
                }
            }
            Vector2Int up = new Vector2Int(nodeX, nodeY + 1);
            if (node.f > 8)
            {
                while (up.y < roads.GetLength(1) && !isIntersection(roads, up))
                {
                    up = new Vector2Int(nodeX, up.y + 1);
                }

            }
            Vector2Int down = new Vector2Int(nodeX, nodeY - 1);
            if (node.f >8)
            {
                while (down.y >= 0 && !isIntersection(roads, down))
                {
                    down = new Vector2Int(nodeX, down.y - 1);
                }
            }
            //left
            if (nodeX - 1 >= 0 && roads[nodeX-1,nodeY] && !parentDict.ContainsKey(left))
            {
                //Debug.Log("Left is valid");
                nodes.Insert(index++, new HeapNode(node.pathCost + 1, left, dest));
                parentDict.Add(left, node.position);
            }
            //right
            if(nodeX + 1 < roads.GetLength(0) && roads[nodeX + 1, nodeY] && !parentDict.ContainsKey(right))
            {
                //Debug.Log("right is valid");
                nodes.Insert(index++, new HeapNode(node.pathCost + 1, right, dest));
                parentDict.Add(right, node.position);
            }//up
            if (nodeY + 1 < roads.GetLength(1) && roads[nodeX,nodeY+1] && !parentDict.ContainsKey(up))
            {
                //Debug.Log("Up is valid");
                nodes.Insert(index++, new HeapNode(node.pathCost + 1, up, dest));
                parentDict.Add(up, node.position);
            }//down
            if (nodeY - 1 >= 0 && roads[nodeX,nodeY-1] && !parentDict.ContainsKey(down))
            {
                //Debug.Log("down is valid");
                nodes.Insert(index++, new HeapNode(node.pathCost + 1, down, dest));
                parentDict.Add(down, node.position);
            }
            index += 1;
        }
        nodes.Clear();
        //Debug.Log("Loop exited");
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        path.Push(origDest);
        while(parentDict.ContainsKey(finalPos))
        {
            if(!roads[finalPos.x,finalPos.y])
            {
                Debug.LogError("HOW ?");
            }
            path.Push(finalPos);
            var temp = finalPos;
            finalPos = parentDict[finalPos];
            parentDict.Remove(temp);
        }
        self.setPositions(path);
        /*
        foreach(Vector2Int posit in path)
        {
            Debug.Log(posit);
        }*/
        //yield return null;
    }
    protected override void Exit()
    {
        base.Exit();
    }
}
