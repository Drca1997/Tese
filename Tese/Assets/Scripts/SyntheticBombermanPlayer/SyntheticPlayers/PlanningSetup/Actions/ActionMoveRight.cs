using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMoveRight : SymbolicAction
{
    public override void Init(PlanningSyntheticPlayer agent)
    {
        Agent = agent;
        RawAction = (int)Action.MoveRight;
        Effect = SyntheticPlayerUtils.deepCopyWorld(agent.GridArray);
    }

    public override void Revert()
    {
        Agent.SimulatedX -= 1;
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
        if (Agent.SimulatedX + 1 < Agent.GridArray.GetLength(0))
        {
            if (Effect[Agent.SimulatedX + 1, Agent.SimulatedY] == (int)Tile.Walkable)
            {
                Effect[Agent.SimulatedX + 1, Agent.SimulatedY] = (int)Tile.Player;
                Agent.SimulatedX += 1;
            }
            else if (Effect[Agent.SimulatedX + 1, Agent.SimulatedY] == (int)Tile.Fire)
            {
                Effect[Agent.SimulatedX + 1, Agent.SimulatedY] = (int)Tile.FireNPlayer;
                Agent.SimulatedX += 1;
            }
            else if (Effect[Agent.SimulatedX + 1, Agent.SimulatedY] == (int)Tile.Bomb)
            {
                Effect[Agent.SimulatedX + 1, Agent.SimulatedY] = (int)Tile.PlayerNBomb;
                Agent.SimulatedX += 1;
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
        if (Agent.SimulatedX + 1 == Agent.position.x && Agent.SimulatedY == Agent.position.y)
        {
            return true;
        }
        return SyntheticPlayerUtils.IsTileWalkableSim(grid, Agent.SimulatedX + 1, Agent.SimulatedY);
    }

    public override bool IsPossible(int [,] grid)
    {
        if (SyntheticPlayerUtils.IsTileWalkable(grid, Agent.position.x + 1, Agent.position.y))
        {
            if (!SyntheticPlayerUtils.IsTileSafe(grid, new int[2] { Agent.position.x, Agent.position.y }) ||
                SyntheticPlayerUtils.IsTileSafe(grid, new int[2] { Agent.position.x + 1, Agent.position.y }))
            {
                return true;
            }
        }
        return false;
    }

}
