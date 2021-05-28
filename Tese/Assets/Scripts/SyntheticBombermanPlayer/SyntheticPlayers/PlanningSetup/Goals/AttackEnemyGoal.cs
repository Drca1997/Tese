using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemyGoal : Goal
{
    public override int[,] GetGoalGrid(int[,] currentGrid, int index, PlanningSyntheticPlayer agent)
    {
        int[,] goalGrid = SyntheticPlayerUtils.deepCopyWorld(currentGrid);

        int[] goalTile = SyntheticPlayerUtils.GetTileFromIndex(index, currentGrid.GetLength(0));
        goalGrid[goalTile[0], goalTile[1]] = (int)Tile.PlayerNBomb;
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


        string result = null;

        for (int i = goalGrid.GetLength(1) - 1; i >= 0; i--)
        {
            result += "\n";
            for (int j = 0; j < goalGrid.GetLength(0); j++)
            {
                result += goalGrid[j, i] + "|";
            }
        }
        Debug.Log("GOAL GRID: " + result);
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
        Debug.Log("Verificando se é possível atacar inimigo");
        if (RefTile != null && PlanningAgent.GridArray[RefTile[0], RefTile[1]] != (int)Tile.AIEnemy) //Update RefTile
        {
            RefTile = null;
        }
        else if (RefTile != null && (PlanningAgent.GridArray[RefTile[0], RefTile[1]]  == (int)Tile.AIEnemy || PlanningAgent.GridArray[RefTile[0], RefTile[1]] == (int)Tile.FireNAIEnemy || PlanningAgent.GridArray[RefTile[0], RefTile[1]] == (int)Tile.FireNBombNAIEnemy)) //Caso RefTile ainda referencie a posição do inimigo
        {
            this.TargetTiles = Utils.GetAdjacentTiles(PlanningAgent.GridArray, RefTile);
            foreach (int[] tile in TargetTiles)
            {
                if (PlanningAgent.GridArray[tile[0], tile[1]] == (int)Tile.Bomb)
                {
                    return false;
                }
            }
        }

        if (RefTile == null) //Caso RefTile seja nula, Procurar por inimigos
        {
            Debug.Log("Procurando por inimigos...");
            GetEnemyPos();
            if (RefTile != null)
            {

                this.TargetTiles = SyntheticPlayerUtils.GetAdjacentTiles(PlanningAgent.GridArray, RefTile);
                foreach (int[] tile in TargetTiles)
                {
                    if (PlanningAgent.GridArray[tile[0], tile[1]] == (int)Tile.Bomb)
                    {
                        return false;
                    }
                }
            }
            else
            {

                return false;
            }


        }
        Debug.Log("Possível atacar inimigo em " + RefTile[0] + ", " + RefTile[1]);
        return true;
    }

    public void GetEnemyPos()
    {
        RefTile = null;
        List<GraphNode> pathToNearestEnemy = NavGraph.GetPath(PlanningAgent.GridArray, PlanningAgent.position.x, PlanningAgent.position.y, this);
        
        if (pathToNearestEnemy != null)
        {
            
            int goalNodeIndex = pathToNearestEnemy[pathToNearestEnemy.Count - 1].Index;
            RefTile = SyntheticPlayerUtils.GetTileFromIndex(goalNodeIndex, PlanningAgent.GridArray.GetLength(0)); ;
        }
        else
        {
            
            RefTile = null;
        }
    }
}
