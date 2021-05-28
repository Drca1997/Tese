using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMoveLeft : SymbolicAction
{
    public override void Init(PlanningSyntheticPlayer agent)
    {
        Agent = agent;
        RawAction = (int)Action.MoveLeft;
        Effect = SyntheticPlayerUtils.deepCopyWorld(agent.GridArray);
    }

    public override void Revert()
    {

        Agent.SimulatedX += 1;

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
        if (Agent.SimulatedX - 1 >= 0)
        {
            if (Effect[Agent.SimulatedX - 1, Agent.SimulatedY] == (int)Tile.Walkable)
            {
                Effect[Agent.SimulatedX - 1, Agent.SimulatedY] = (int)Tile.Player;
                Agent.SimulatedX -= 1;
            }
            else if (Effect[Agent.SimulatedX - 1, Agent.SimulatedY] == (int)Tile.Fire)
            {
                Effect[Agent.SimulatedX - 1, Agent.SimulatedY] = (int)Tile.FireNPlayer;
                Agent.SimulatedX -= 1;
            }
            else if (Effect[Agent.SimulatedX - 1, Agent.SimulatedY] == (int)Tile.Bomb)
            {
                Effect[Agent.SimulatedX - 1, Agent.SimulatedY] = (int)Tile.PlayerNBomb;
                Agent.SimulatedX -= 1;
            }
        }

    }

    public override bool CheckPreconditions(int [,] grid)
    {
        if (grid[Agent.SimulatedX, Agent.SimulatedY] == (int)Tile.PlayerNBomb)
        {
            return false;
        }
        if (Agent.SimulatedX - 1 == Agent.position.x && Agent.SimulatedY == Agent.position.y)
        {
            return true;
        }
        return SyntheticPlayerUtils.IsTileWalkableSim(grid, Agent.SimulatedX - 1, Agent.SimulatedY);
    }

    public override bool IsPossible(int [,] grid)
    {
        return SyntheticPlayerUtils.IsTileWalkable(grid, Agent.position.x - 1, Agent.position.y);
    }
}