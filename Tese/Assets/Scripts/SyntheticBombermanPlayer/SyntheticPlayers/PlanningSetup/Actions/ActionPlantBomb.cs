using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlantBomb : SymbolicAction
{
    //Initializes the attributes of the action
    public override void Init(PlanningSyntheticPlayer agent)
    {
        Agent = agent;
        RawAction = (int)Action.PlantBomb;
        Effect = SyntheticPlayerUtils.deepCopyWorld(agent.GridArray);
    }

    //Reverts the effects of the action. It is like it never happened
    public override void Revert()
    {
        //Agent.SimulatedPlantedBomb = false;
        Effect[Agent.SimulatedX, Agent.SimulatedY] = (int)Tile.PlayerNBomb;
    }


    //Simulates the action in the environment, applying its effects
    public override void Simulate()
    {
        Effect[Agent.SimulatedX, Agent.SimulatedY] = (int)Tile.Player;
    }

    //Checks if the action is possible to be simulated
    public override bool CheckPreconditions(int [,] grid)
    {
        return grid[Agent.SimulatedX, Agent.SimulatedY] == (int)Tile.PlayerNBomb;
    }

    //Checks if the action is possible to be executed in the current game state
    public override bool IsPossible(int [,] grid)
    {
        //if (!Agent.SimulatedPlantedBomb)
        if (grid[Agent.position.x, Agent.position.y] != (int)Tile.PlayerNBomb)
        {
            return true;
        }
        return false;
    }
}
