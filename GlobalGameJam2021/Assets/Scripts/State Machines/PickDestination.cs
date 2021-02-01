using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/PickDestination")]
public class PickDestination : BaseState
{
    private Vector2Int possibleDestinations;
    public MapGenerator mapStance;
    protected override void childEnter(ThiefAI cur)
    {
        mapStance = MapGenerator.instance;
    }
    private void PickThiefDestination()
    {
        HashSet<Vector2Int> visited = self.getVisitedLocations();

        if (visited.Count == mapStance.destinations.Count)
        {
            self.setVisitedEverything();
            Exit();
        }
        List<Vector2Int> visitable = new List<Vector2Int>();
        foreach (var dest in mapStance.destinations)
        {
            if (!visited.Contains(dest.Key))
            {
                visitable.Add(dest.Key);
            }
        }
        int locInt = Random.Range(0, visitable.Count);
        int curIndex = 0;
        foreach (var loc in visitable)
        {
            if (curIndex == locInt)
            {
                self.setDestination(loc, mapStance.destinations[loc]);
                self.visitLocation(loc);
                break;
            }
            curIndex++;
        }
    }
    public override IEnumerator Perform()
    {
        yield return null;
        bool shouldThiefSteal = self.justStolen ? Random.Range(0f, 1f) > 0.9f : Random.Range(0f, 1f) > 0.2f;
        if (self.isThief && shouldThiefSteal)
        {
            PickThiefDestination();
        }
        else
        {
            bool[,] notAllowed = mapStance.roads;
            int randomI = Random.Range(0, notAllowed.GetLength(0));
            int randomJ = Random.Range(0, notAllowed.GetLength(1));
            Vector2Int loc = new Vector2Int(randomI, randomJ);
            while (!mapStance.normalBuildings.ContainsKey(loc) && !mapStance.destinations.ContainsKey(loc))
            {
                randomI = Random.Range(0, notAllowed.GetLength(0));
                randomJ = Random.Range(0, notAllowed.GetLength(1));
                loc = new Vector2Int(randomI, randomJ);
            }
            if (mapStance.normalBuildings.ContainsKey(loc))
            {
                self.setDestination(loc, mapStance.normalBuildings[loc]);
            }
            else
            {
                self.setDestination(loc, mapStance.destinations[loc]);
            }
            self.visitLocation(loc);
        }
        Exit();
    }

    protected override void Exit()
    {
        if (nextState && !self.hasVisitedEverything())
            self.changeState(nextState);
    }
}
