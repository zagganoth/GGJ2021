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
        public float heuristic;
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
    private bool validatePosition(Vector2Int position, bool[,] allowedPositions, Dictionary<Vector2Int, Vector2Int> parentDict)
    {
        return position.y >= 0 && position.y < allowedPositions.GetLength(1) && position.x >= 0 && position.x < allowedPositions.GetLength(0)
            && allowedPositions[position.x, position.y] && !parentDict.ContainsKey(position);
    } 
    public override IEnumerator Perform()
    {
        yield return null;

        parentDict = new Dictionary<Vector2Int, Vector2Int>();
        Vector2Int dest = self.getDestination();
        Vector2Int origDest = dest;
        Vector3 pos = self.getPos();
        bool[,] roads = mapStance.roads;
        if (roads.Length == 0)
        {
            Exit();
        }
        dest = getAdjacentRoad(roads, dest);
        Vector2Int convertedPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
        PriorityQueue<HeapNode> nodes = new PriorityQueue<HeapNode>(1000);
        nodes.Insert(1, new HeapNode(0, convertedPos,dest));
        Vector2Int finalPos = dest;
        int index = 0;
        while(nodes.Count > 0)
        {
            if(index > 500)
            {
                Debug.Log("Too many iterations needed to determine pathing for this AI. There is probably a bug causing this");
                break;
            }
            var node = nodes.Pop();
            int nodeX = node.position.x;
            int nodeY = node.position.y;
            if(node.position == dest)
            {
                finalPos = node.position;
                break;
            }
            Vector2Int[] adjacentPositions = {
                new Vector2Int(nodeX - 1, nodeY), //left
                new Vector2Int(nodeX + 1, nodeY), //right
                new Vector2Int(nodeX, nodeY + 1), //up
                new Vector2Int(nodeX, nodeY - 1) //down
            };
            var translations = new[] {
                (-1,0),//left
                (+1,0),//right
                (0,+1),//up
                (0,-1)//down
            };
            for (int posIndex = 0; posIndex < adjacentPositions.Length; posIndex++) {      
                if (node.heuristic > 8) // 
                {
                    while (validatePosition(adjacentPositions[posIndex], roads, parentDict) && !isIntersection(roads, adjacentPositions[posIndex]))
                    {
                        adjacentPositions[posIndex] = new Vector2Int(adjacentPositions[posIndex].x + translations[posIndex].Item1, adjacentPositions[posIndex].y + translations[posIndex].Item2);
                    }
                }
                if (validatePosition(adjacentPositions[posIndex], roads, parentDict))
                {
                    nodes.Insert(index++, new HeapNode(node.pathCost + 1, adjacentPositions[posIndex], dest));
                    parentDict.Add(adjacentPositions[posIndex], node.position);
                }
            }
            index += 1;
        }
        nodes.Clear();
        Debug.Log("Loop exited");
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        path.Push(dest);
        while(parentDict.ContainsKey(finalPos))
        {
            if(!roads[finalPos.x,finalPos.y])
            {
                Debug.LogError("Parent not found for something on path. Something is very wrong.");
            }
            path.Push(finalPos);
            var temp = finalPos;
            finalPos = parentDict[finalPos];
            parentDict.Remove(temp);
        }
        self.setPositions(path);
    }
    protected override void Exit()
    {
        base.Exit();
    }
}
