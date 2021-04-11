using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemyGoal : Goal
{
    
    public override void Init()
    {
       
        this.Agent = gameObject.GetComponent<PlanningAgent>();
        this.GameWorld = Agent.Grid.Array;
        GetEnemyPos();
        IsPossible();
        
    }
    public override bool IsPossible()
    {
        Debug.Log("Verificando se é possível atacar inimigo");
        if (RefTile != null)
        {
            this.TargetTiles = Utils.GetAdjacentTiles(GameWorld, RefTile);
            foreach (int[] tile in TargetTiles)
            {
                if (GameWorld[tile[0], tile[1]] == 4)
                {
                    return false;
                }
            }
        }
        else
        {
            Debug.Log("Procurando por inimigos...");
            GetEnemyPos();
            if (RefTile != null)
            {
               
                this.TargetTiles = Utils.GetAdjacentTiles(GameWorld, RefTile);
                foreach (int[] tile in TargetTiles)
                {
                    if (GameWorld[tile[0], tile[1]] == 4)
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

    public override double Heuristic(ActionStateGraphNode state, ActionStateGraphNode goal)
    {
        int[] start = new int[2] { state.Agent.SimulatedX, goal.Agent.SimulatedY };
        int[] end = new int[2] { goal.Agent.X, goal.Agent.Y };
        return AStar.CalculateManhattanDistance(start, end);
    }

    public override bool IsObjective(ActionStateGraphNode node)
    {
        
        if (node.Agent.SimulatedX== node.Agent.X && node.Agent.SimulatedY == node.Agent.Y)
        {
            
            foreach (int[] tile in TargetTiles)
            {
                
                if (node.Grid[tile[0], tile[1]] == 4 || node.Grid[tile[0], tile[1]] == 5)
                {
                    return true;
                }
            }
        }    
        return false;
    }

    public override int[,] GetGoalGrid(int[,] currentGrid, int index, PlanningAgent agent)
    {
        int[,] goalGrid = Utils.deepCopyWorld(currentGrid);
       
        int[] goalTile = Utils.GetTileFromIndex(index, currentGrid.GetLength(0));
        goalGrid[goalTile[0], goalTile[1]] = 5;
        if (goalGrid[agent.X, agent.Y] == 0)
        {
            goalGrid[agent.X, agent.Y] = 1;
        }
        else if (goalGrid[agent.X, agent.Y] == 5)
        {
            goalGrid[agent.X, agent.Y] = 4;
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
            
        return goalGrid;
    }

    public void GetEnemyPos()
    {
        RefTile = null;
        List<GraphNode> pathToNearestEnemy = NavigationGraph.GetPath(GameWorld, Agent.X, Agent.Y, this);
        if (pathToNearestEnemy != null)
        {

            int goalNodeIndex = pathToNearestEnemy[pathToNearestEnemy.Count - 1].Index;
            RefTile = Utils.GetTileFromIndex(goalNodeIndex, Agent.Grid.Array.GetLength(0)); ;
            Debug.Log("Encontrado inimigo em " + RefTile[0] + ", " + RefTile[1]);
          
        }
        else
        {
            Debug.Log("Nenhum inimigo encontrado!");
            RefTile = null;
        }
    }
}
