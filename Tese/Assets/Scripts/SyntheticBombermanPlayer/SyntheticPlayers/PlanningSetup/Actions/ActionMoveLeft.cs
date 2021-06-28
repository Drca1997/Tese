using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMoveLeft : SymbolicAction
{
    //Initializes the attributes of the action
    public override void Init(PlanningSyntheticPlayer agent)
    {
        Agent = agent;
        RawAction = (int)Action.MoveLeft;
        Effect = SyntheticPlayerUtils.deepCopyWorld(agent.GridArray);
    }

    //Reverts the effects of the action. It is like it never happened
    public override void Revert()
    {

        Agent.SimulatedX += 1;

    }

    //Simulates the action in the environment, applying its effects
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

    //Checks if the action is possible to be simulated
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

    //Checks if the action is possible to be executed in the current game state
    public override bool IsPossible(int [,] grid)
    {
        if (SyntheticPlayerUtils.IsTileWalkable(grid, Agent.position.x - 1, Agent.position.y))
        {
            if (!SyntheticPlayerUtils.IsTileSafe(grid, new int[2] { Agent.position.x, Agent.position.y }) || 
                SyntheticPlayerUtils.IsTileSafe(grid, new int[2] { Agent.position.x - 1, Agent.position.y}))
            {
                Debug.Log("SAFE TILE: " + Agent.position.x + "," + Agent.position.y);
                return true;
            }
        }
        return false;
    }
}
