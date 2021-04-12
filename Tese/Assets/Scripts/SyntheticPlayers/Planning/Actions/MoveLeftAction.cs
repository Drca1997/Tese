using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeftAction : Action
{
    public override void Revert()
    {
        
        Agent.SimulatedX += 1;
        
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
        if (Agent.SimulatedX - 1 >= 0)
        {
            Debug.Log(Effect[Agent.SimulatedX - 1, Agent.SimulatedY]);
            if (Effect[Agent.SimulatedX-1, Agent.SimulatedY] == 4)
            {
                Effect[Agent.SimulatedX-1, Agent.SimulatedY] = 5;
                Agent.SimulatedX -= 1;
                
            }
            else if (Effect[Agent.SimulatedX-1, Agent.SimulatedY] == 1)
            {
                Effect[Agent.SimulatedX-1, Agent.SimulatedY] = 0;
                Agent.SimulatedX -= 1;

            }
        }
        
    }

    public override bool CheckPreconditions()
    {
        if (Agent.SimulatedX - 1 == Agent.X && Agent.SimulatedY == Agent.Y)
        {
            return true;
        }
        return Utils.IsTileWalkable(Agent.Grid, Agent.SimulatedX-1, Agent.SimulatedY);
    }

    public override bool IsPossible()
    {
        return CheckPreconditions();
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
   if (agent.X - 1 >= 0)
   {
       if (grid.Array[agent.X - 1, agent.Y] == 4)
       {
           grid.Array[agent.X - 1, agent.Y] = 5;
       }
       else if (grid.Array[agent.X - 1, agent.Y] == 1)
       {
           grid.Array[agent.X - 1, agent.Y] = 0;
       }
   }
}*/

}
