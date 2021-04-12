using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAgent : BaseAgent, IDecisionRequester
{
    public void GetWorld(Grid grid, int x, int y)
    {
        this.Grid = grid;
        this.x = x;
        this.y = y;
    }

    public int RequestDecision()
    {
        return 5;
    }

}
