using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDoNothing : SymbolicAction
{
    //Initializes the attributes of the action
    public override void Init(PlanningSyntheticPlayer agent)
    {
        Agent = agent;
        RawAction = (int)Action.PlantBomb;
        Effect = SyntheticPlayerUtils.deepCopyWorld(agent.GridArray);
    }

    //Checks if the action is possible to be simulated
    public override bool CheckPreconditions(int[,] grid)
    {
        if (grid[Agent.SimulatedX, Agent.SimulatedY] == (int)Tile.PlayerNBomb)
        {
            return false;
        }
        return true;
    }

    //Checks if the action is possible to be executed in the current game state
    public override bool IsPossible(int [,] grid)
    {
        return true;
    }

    //Reverts the effects of the action. It is like it never happened
    public override void Revert()
    {
        
    }

    //Simulates the action in the environment, applying its effects
    public override void Simulate()
    {
        
    }
}
