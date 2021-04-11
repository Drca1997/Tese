using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDownAction : Action
{

    public override void Revert()
    {
        Agent.SimulatedY += 1;
    }
    public override void Simulate()
    {
        if (Effect[Agent.SimulatedX, Agent.SimulatedY] == 5)
        {
            Effect[Agent.SimulatedX, Agent.SimulatedY] = 4;
        }
        else if (Effect[Agent.SimulatedX, Agent.SimulatedY] == 0)
        {
            Effect[Agent.SimulatedX, Agent.SimulatedY] = 1;
        }
        if (Agent.SimulatedY - 1 >= 0)
        {
            if (Effect[Agent.SimulatedX, Agent.SimulatedY-1] == 4)
            {
                Effect[Agent.SimulatedX, Agent.SimulatedY-1] = 5;
                Agent.SimulatedY -= 1;
            }
            else if (Effect[Agent.SimulatedX, Agent.SimulatedY-1] == 1)
            {
                Effect[Agent.SimulatedX, Agent.SimulatedY-1] = 0;
                Agent.SimulatedY -= 1;
            }
        }
    }

    public override bool CheckPreconditions()
    {
        if (Agent.SimulatedX == Agent.X && Agent.SimulatedY - 1 == Agent.Y)
        {
            return true;
        }
        return Utils.IsTileWalkable(Agent.Grid, Agent.SimulatedX, Agent.SimulatedY - 1);
    }
    /**
    public override void Execute(Grid grid, BaseAgent agent)
    {
        if (grid.Array[agent.X, agent.Y] == 5)
        {
            grid.Array[agent.X, agent.Y] = 4;
        }
        else if (grid.Array[agent.X, agent.Y] == 0)
        {
            grid.Array[agent.X, agent.Y] = 1;
        }
        if (agent.Y - 1 >= 0)
        {
            if (grid.Array[agent.X, agent.Y-1] == 4)
            {
                grid.Array[agent.X, agent.Y-1] = 5;
            }
            else if (grid.Array[agent.X, agent.Y-1] == 1)
            {
                grid.Array[agent.X, agent.Y-1] = 0;
            }
        }
    }*/
}
