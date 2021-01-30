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

    public override IEnumerator Perform()
    {
        yield return null;
        int locInt = Random.Range(0, mapStance.destinationLocations.Count);
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
        if (nextState)
            self.changeState(nextState);
    }
}
