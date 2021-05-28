using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDoNothing : SymbolicAction
{
    public override void Init(PlanningSyntheticPlayer agent)
    {
        Agent = agent;
        RawAction = (int)Action.PlantBomb;
        Effect = SyntheticPlayerUtils.deepCopyWorld(agent.GridArray);
    }

    public override bool CheckPreconditions(int[,] grid)
    {
        if (grid[Agent.SimulatedX, Agent.SimulatedY] == (int)Tile.PlayerNBomb)
        {
            return false;
        }
        return true;
    }

    public override bool IsPossible(int [,] grid)
    {
        return true;
    }

    public override void Revert()
    {
        
    }

    public override void Simulate()
    {
        
    }
}
