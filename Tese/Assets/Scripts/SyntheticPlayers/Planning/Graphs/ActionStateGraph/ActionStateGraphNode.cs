using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionStateGraphNode : GraphNode
{
    private int[,] grid;
    private PlanningAgent agent;
    private int[] agentPos;
    private Action actionTakenToReachHere;

    public ActionStateGraphNode(int index, PlanningAgent agent, int[,] grid) : base(index)
    {
        
        this.Grid = Utils.deepCopyWorld(grid);
       
        this.Agent = agent;
        AgentPos = new int[2] {agent.SimulatedX, agent.SimulatedY };
    }


    public string DebugWorld()
    {
        string result = null;
       
        for (int i = grid.GetLength(1) - 1; i >= 0; i--)
        {
            result += "\n";   
            for (int j=0; j < grid.GetLength(0); j++)
            {
                result += grid[j, i] + "|";
            }
        }
        return result;
    }
    public PlanningAgent Agent { get => agent; set => agent = value; }
    public Action ActionTakenToReachHere { get => actionTakenToReachHere; set => actionTakenToReachHere = value; }
    public int[,] Grid { get => grid; set => grid = value; }
    public int[] AgentPos { get => agentPos; set => agentPos = value; }

}
