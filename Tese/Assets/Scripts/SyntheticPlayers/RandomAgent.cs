using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAgent : BaseAgent, IDecisionRequester
{

    public void GetWorld(Grid grid, int x, int y)
    {
        this.Grid = grid;
        this.x = x;
        this.y = y;
    }

    
    public int RequestDecision()
    {
        int action = Random.Range(0, 6);
        while (!Utils.IsValidAction(Grid, this, action))
        {
            action = Random.Range(0, 6);
        }
        Debug.Log(gameObject.name + " " + Utils.ActionToString(action));
        return action;
    }


}
