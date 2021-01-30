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
        Debug.Log(mapStance);
    }

    public override IEnumerator Perform()
    {
        yield return null;
        int attempts =0;
        int locInt = Random.Range(0, mapStance.destinationLocations.Count);
        HashSet<int> visited = self.getVisitedLocations();
        if(visited.Count == mapStance.destinationLocations.Count)
        {
            self.setVisitedEverything();
            Exit();
        }
        while(visited.Contains(locInt))
        {
            locInt = Random.Range(0, mapStance.destinationLocations.Count);
            attempts++;
            if(attempts > mapStance.destinationLocations.Count * 2)
            {
                self.setVisitedEverything();
                Exit();
            }
        } 
        int curIndex = 0;
        foreach(var loc in mapStance.destinationLocations)
        {
            if (curIndex == locInt)
            {
                self.setDestination(loc);
                break;
            }
            curIndex++;
        }
        Exit();
    }

    protected override void Exit()
    {
        if (nextState && !self.hasVisitedEverything())
            self.changeState(nextState);
    }
}
