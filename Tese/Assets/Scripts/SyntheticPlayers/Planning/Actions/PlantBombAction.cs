using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBombAction : Action
{
    
    public override void Revert()
    {
        Agent.SimulatedPlantedBomb = false;
    }

    public override void Simulate()
    {
        Effect[Agent.SimulatedX, Agent.SimulatedY] = 5;
        Agent.SimulatedPlantedBomb = true;
    }

    public override bool CheckPreconditions()
    {
        if (!Agent.SimulatedPlantedBomb && Agent.Grid.Array[Agent.SimulatedX, Agent.SimulatedY] != 5)
        {
            return true;
        }
        return false;
    }

    /*
    public override void Execute(Grid grid, BaseAgent agent)
    {
        grid.Array[agent.X, agent.Y] = 5;
    }*/
}
