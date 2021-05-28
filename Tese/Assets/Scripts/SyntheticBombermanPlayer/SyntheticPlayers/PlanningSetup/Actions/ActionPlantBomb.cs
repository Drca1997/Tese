using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlantBomb : SymbolicAction
{

    public override void Init(PlanningSyntheticPlayer agent)
    {
        Agent = agent;
        RawAction = (int)Action.PlantBomb;
        Effect = SyntheticPlayerUtils.deepCopyWorld(agent.GridArray);
    }

    public override void Revert()
    {
        //Agent.SimulatedPlantedBomb = false;
        Effect[Agent.SimulatedX, Agent.SimulatedY] = (int)Tile.PlayerNBomb;
    }

    public override void Simulate()
    {
        Effect[Agent.SimulatedX, Agent.SimulatedY] = (int)Tile.Player;
    }
    /*
    public void Execute()
    {
        Effect[Agent.SimulatedX, Agent.SimulatedY] = (int)Tile.PlayerNBomb;
        //Agent.SimulatedPlantedBomb = true;
    }*/

    public override bool CheckPreconditions(int [,] grid)
    {
        return grid[Agent.SimulatedX, Agent.SimulatedY] == (int)Tile.PlayerNBomb;
    }

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
