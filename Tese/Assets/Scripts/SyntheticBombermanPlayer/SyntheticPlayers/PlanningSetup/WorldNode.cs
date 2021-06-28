using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNode : GraphNode
{
    private int[,] grid; //grid of the game state represented by this node
    private PlanningSyntheticPlayer agent; //reference to the planning agent
    private int[] agentPos; //planning agent's position

    //Action taken to reach this node. This is useful when tracing back to the starting node 
    //in the end of A* in order to get the full path of actions
    private SymbolicAction actionTakenToReachHere;

    /* Constructor
     * int index: index of the node. This is not used.
     * PlanningSyntheticPlayer agent: the planning agent
     * int[,] grid: grid that depicts a certain game state
     * */
    public WorldNode(int index, PlanningSyntheticPlayer agent, int[,] grid) : base(index)
    {

        this.Grid = SyntheticPlayerUtils.deepCopyWorld(grid);

        this.Agent = agent;
        AgentPos = new int[2] { agent.SimulatedX, agent.SimulatedY };
    }

    //Prints the game state depicted by this node
    public string DebugWorld()
    {
        string result = null;

        for (int i = grid.GetLength(1) - 1; i >= 0; i--)
        {
            result += "\n";
            for (int j = 0; j < grid.GetLength(0); j++)
            {
                result += grid[j, i] + "|";
            }
        }
        return result;
    }
    public PlanningSyntheticPlayer Agent { get => agent; set => agent = value; }
    public SymbolicAction ActionTakenToReachHere { get => actionTakenToReachHere; set => actionTakenToReachHere = value; }
    public int[,] Grid { get => grid; set => grid = value; }
    public int[] AgentPos { get => agentPos; set => agentPos = value; }
}
