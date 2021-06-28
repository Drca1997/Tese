using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeSafeGoal : Goal 
{
    /* Returns the grid that represents the goal game state
    * int[,] currentGrid: grid that represents the current game state
    * int index: Index of the Goal Tile (tile where the agent wants to be)
    * PlanningSyntheticPlayer agent: reference to the planning agent
    */
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

    /*Provides a Custom Heuristic to be used in A*. This particular case is the Manhattan Distance
     * WorldNode state: Current Node 
     * WorldNode goal: Goal Node
     **/
    public override double Heuristic(WorldNode state, WorldNode goal)
    {
        int[] start = new int[2] { state.Agent.SimulatedX, goal.Agent.SimulatedY };
        int[] end = new int[2] { goal.Agent.position.x, goal.Agent.position.y };
        return SyntheticPlayerUtils.CalculateManhattanDistance(start, end);
    }

    /*
     * Checks if a particular node is the goal node
     * WorldNode node: Node to be checked
     */
    public override bool IsObjective(WorldNode node)
    {
        return node.Agent.SimulatedX == node.Agent.position.x && node.Agent.SimulatedY == node.Agent.position.y;
    }

    //Checks if the Goal is Possible to Execute
    public override bool IsPossible()
    {

        //Debug.Log("Verificando se é possível fugir de uma bomba");
        RefTile = new int[2] { PlanningAgent.position.x, PlanningAgent.position.y };

        if (!SyntheticPlayerUtils.IsTileSafe(PlanningAgent.GridArray, RefTile))
        {
            //Debug.Log("NOT SAFE! POSSÍVEL FUGIR DE BOMBA!");
            TargetTiles = SyntheticPlayerUtils.dangerTiles(SyntheticPlayerUtils.dangerMap(PlanningAgent.GridArray), true);
            return true;
        }
        //Debug.Log("Já está seguro. Mais produtivo encontrar outro objetivo...");

        return false;
    }
}
