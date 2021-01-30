using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseState : ScriptableObject
{
    [SerializeField]
    protected BaseState nextState;
    protected ThiefAI self;
    protected virtual void childEnter(ThiefAI cur)
    {

    }

    public void Enter(ThiefAI cur)
    {
        self = cur;
        childEnter(cur);
        //Perform();
    }
    
    public virtual IEnumerator Perform()
    {
        yield return null;
        Exit();
    }

    protected virtual void Exit()
    {
        if(nextState)
            nextState.Enter(self);
    }

}
