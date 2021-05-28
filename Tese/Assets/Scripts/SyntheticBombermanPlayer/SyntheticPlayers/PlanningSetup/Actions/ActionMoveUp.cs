using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMoveUp : SymbolicAction
{
    public override void Init(PlanningSyntheticPlayer agent)
    {
        Agent = agent;
        RawAction = (int)Action.MoveUp;
        Effect = SyntheticPlayerUtils.deepCopyWorld(agent.GridArray);
    }

    public override void Revert()
    {
        Agent.SimulatedY -= 1;

    }

    public override void Simulate()
    {
        if (Effect[Agent.SimulatedX, Agent.SimulatedY] == (int)Tile.PlayerNBomb)
        {
            Effect[Agent.SimulatedX, Agent.SimulatedY] = (int)Tile.Bomb;
        }
        else if (Effect[Agent.SimulatedX, Agent.SimulatedY] == (int)Tile.Player)
        {
            Effect[Agent.SimulatedX, Agent.SimulatedY] = (int)Tile.Walkable;
        }
        if (Agent.SimulatedY + 1 < Agent.GridArray.GetLength(1))
        {
            if (Effect[Agent.SimulatedX, Agent.SimulatedY + 1] == (int)Tile.Walkable)
            {
                Effect[Agent.SimulatedX, Agent.SimulatedY + 1] = (int)Tile.Player;
                Agent.SimulatedY += 1;
            }
            else if (Effect[Agent.SimulatedX, Agent.SimulatedY + 1] == (int)Tile.Fire)
            {
                Effect[Agent.SimulatedX, Agent.SimulatedY + 1] = (int)Tile.FireNPlayer;
                Agent.SimulatedY += 1;
            }
            else if (Effect[Agent.SimulatedX, Agent.SimulatedY + 1] == (int)Tile.Bomb)
            {
                Effect[Agent.SimulatedX, Agent.SimulatedY + 1] = (int)Tile.PlayerNBomb;
                Agent.SimulatedY += 1;
            }
        }
    }

    public override bool CheckPreconditions(int [,] grid)
    {

        //Debug.Log("PRECOND SIMPOS: " + Agent.SimulatedX + ", " + Agent.SimulatedY + "= " + grid[Agent.SimulatedX, Agent.SimulatedY]);
        if (grid[Agent.SimulatedX, Agent.SimulatedY] == (int)Tile.PlayerNBomb)
        {
            return false;
        }
       
        if (Agent.SimulatedX == Agent.position.x && Agent.SimulatedY + 1 == Agent.position.y)
        {
            return true;
        }
        
        return SyntheticPlayerUtils.IsTileWalkableSim(grid, Agent.SimulatedX, Agent.SimulatedY + 1);
    }

    public override bool IsPossible(int [,] grid)
    {
        if (SyntheticPlayerUtils.IsTileWalkable(grid, Agent.position.x, Agent.position.y + 1))
        {
            if (SyntheticPlayerUtils.IsTileSafe(grid, new int[2] { Agent.position.x, Agent.position.y + 1}))
            {
                return true;
            }
        }
        return false;
    }
}
