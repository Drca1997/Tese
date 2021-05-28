using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeSafeGoal : Goal 
{
    public override int[,] GetGoalGrid(int[,] currentGrid, int index, PlanningSyntheticPlayer agent)
    { 
        int[,] goalGrid = SyntheticPlayerUtils.deepCopyWorld(currentGrid);
        int[] goalTile = SyntheticPlayerUtils.GetTileFromIndex(index, currentGrid.GetLength(0));
        goalGrid[goalTile[0], goalTile[1]] = (int)Tile.Player;
        if (goalGrid[agent.position.x, agent.position.y] == (int)Tile.Player)
        {
            goalGrid[agent.position.x, agent.position.y] = (int)Tile.Walkable;
        }
        else if (goalGrid[agent.position.x, agent.position.y] == (int)Tile.PlayerNBomb)
        {
            goalGrid[agent.position.x, agent.position.y] = (int)Tile.Bomb;
        }
        agent.SimulatedX = goalTile[0];
        agent.SimulatedY = goalTile[1];
        return goalGrid;
    
    }

    public override double Heuristic(WorldNode state, WorldNode goal)
    {
        int[] start = new int[2] { state.Agent.SimulatedX, goal.Agent.SimulatedY };
        int[] end = new int[2] { goal.Agent.position.x, goal.Agent.position.y };
        return SyntheticPlayerUtils.CalculateManhattanDistance(start, end);
    }

    public override bool IsObjective(WorldNode node)
    {
        return node.Agent.SimulatedX == node.Agent.position.x && node.Agent.SimulatedY == node.Agent.position.y;
    }

    public override bool IsPossible()
    {

        Debug.Log("Verificando se é possível fugir de uma bomba");
        RefTile = new int[2] { PlanningAgent.position.x, PlanningAgent.position.y };

        if (!SyntheticPlayerUtils.IsTileSafe(PlanningAgent.GridArray, RefTile))
        {
            Debug.Log("NOT SAFE! POSSÍVEL FUGIR DE BOMBA!");
            TargetTiles = SyntheticPlayerUtils.dangerTiles(SyntheticPlayerUtils.dangerMap(PlanningAgent.GridArray), true);
            return true;
        }
        Debug.Log("Já está seguro. Mais produtivo encontrar outro objetivo...");

        return false;
    }
}
