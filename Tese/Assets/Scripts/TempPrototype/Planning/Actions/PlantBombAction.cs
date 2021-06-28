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
        Effect[Agent.SimulatedX, Agent.SimulatedY] = 0;
    }
    public void Execute()
    {
        Effect[Agent.SimulatedX, Agent.SimulatedY] = 5;
        Agent.SimulatedPlantedBomb = true;
    }

    public override bool CheckPreconditions()
    {
        return Effect[Agent.SimulatedX, Agent.SimulatedY] == 5;
    }

    public override bool IsPossible()
    {
        if (!Agent.PlantedBomb && Agent.Grid.Array[Agent.X, Agent.Y] != 5)
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
