using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBeSafe : GoalTemplate
{

    public override void Init()
    {

    
        this.Agent = gameObject.GetComponent<PlanningAgent>();
        this.GameWorld = Agent.Grid.Array;
        this.TargetTiles = null;
        IsPossible();
    }
   
    public override bool IsPossible()
    {
        Debug.Log("Verificando se é possível fugir de uma bomba");
        this.RefTile = new int[2] {Agent.X, Agent.Y };
        
        if (!Utils.IsTileSafe(this.GameWorld, RefTile))
        {
            Debug.Log("NOT SAFE! POSSÍVEL FUGIR DE BOMBA!");
            this.TargetTiles = Utils.dangerTiles(Utils.dangerMap(Agent.Grid.Array), true);
            return true;
        }
        Debug.Log("Já está seguro. Mais produtivo encontrar outro objetivo...");
    
        return false;
    }

    public override double Heuristic(ActionStateGraphNode state, ActionStateGraphNode goal)
    {

        int[] start = new int[2] {state.Agent.SimulatedX, goal.Agent.SimulatedY };
        int[] end = new int[2] { goal.Agent.X, goal.Agent.Y };
        return AStarClass.CalculateManhattanDistance(start, end);

    }

    public override bool IsObjective(ActionStateGraphNode node)
    {
        if (node.Agent.SimulatedX == node.Agent.X && node.Agent.SimulatedY == node.Agent.Y)
        {
            return true;
        }
        return false;

    }

    public override int[,] GetGoalGrid(int[,] currentGrid, int index, PlanningAgent agent)
    {
        int[,] goalGrid = Utils.deepCopyWorld(currentGrid);
        int[] goalTile = Utils.GetTileFromIndex(index, currentGrid.GetLength(0));
        goalGrid[goalTile[0], goalTile[1]] = 0;
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
        return goalGrid;
    }

}
