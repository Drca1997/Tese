using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNode : GraphNode
{
    private int[,] grid;
    private PlanningSyntheticPlayer agent;
    private int[] agentPos;
    private SymbolicAction actionTakenToReachHere;

    public WorldNode(int index, PlanningSyntheticPlayer agent, int[,] grid) : base(index)
    {

        this.Grid = SyntheticPlayerUtils.deepCopyWorld(grid);

        this.Agent = agent;
        AgentPos = new int[2] { agent.SimulatedX, agent.SimulatedY };
    }


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
