using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/DeterminePath")]
public class DeterminePath : BaseState
{
    public class HeapNode : IComparable
    {

        Vector2Int position;
        Vector2Int dest;
        public HeapNode(int depth, Vector2Int destination, Vector2Int curPos)
        {
            position = curPos;
            dest = destination;
        }
        public int CompareTo(object obj)
        { 
            HeapNode other = obj as HeapNode;
            return -1;

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
    public override IEnumerator Perform()
    {
        yield return null;
        Vector2Int dest = self.getDestination();
        Vector3 pos = self.getPos();
        bool[,] roads = mapStance.roads;

        //Tuple<Vector2Int, Vector2Int> node;
        PriorityQueue<(int, Vector2Int)> nodes = new PriorityQueue<(int, Vector2Int)>(1000);
        nodes.Insert(1, (0, new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z))));
        while(nodes.Count > 0)
        {
            var node = nodes.Pop();
            int nodeX = node.Item2.x;
            int nodeY = node.Item2.y;
            //right
            if(nodeX - 1 > 0)
            {
                nodes.Insert(1, (node.Item1 + 1, new Vector2Int(nodeX - 1, nodeY)));
            }
            if(nodeX + 1 < roads.GetLength(0))
            {
                nodes.Insert(1, (node.Item1 + 1, new Vector2Int(nodeX + 1, nodeY)));
            }
            if (nodeY + 1 < roads.GetLength(1))
            {
                nodes.Insert(1, (node.Item1 + 1, new Vector2Int(nodeX, nodeY+1)));
            }
            if (nodeY - 1 > 0)
            {
                nodes.Insert(1, (node.Item1 + 1, new Vector2Int(nodeX, nodeY - 1)));
            }
        }
        Debug.Log(nodes.Top());
    }
    protected override void Exit()
    {
        base.Exit();
    }
}
