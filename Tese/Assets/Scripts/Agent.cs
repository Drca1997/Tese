using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent 
{
    public string typeName;
    public Vector2Int position;
    public Vector2Int[] relative_sensors;
    public Vector2Int[] constant_sensors;
    public List<int> states;

    public abstract Grid UpdateAgent(Grid g, int step_stage);

    public Agent[] GetSensors(Grid grid)
    {
        int numberSensors = relative_sensors.Length + constant_sensors.Length;

        Agent[] agentSensors = new Agent[numberSensors];
        int i = 0;

        foreach (Vector2Int sensorPos in relative_sensors)
        {
            Vector2Int realPos = GetRealPos(position, sensorPos, grid);
            agentSensors[i] = grid.agentGrid[realPos.x, realPos.y];
            i++;
        }

        foreach (Vector2Int sensorPos in constant_sensors)
        {
            Vector2Int realPos = GetRealPos(position, sensorPos, grid);
            agentSensors[i] = grid.agentGrid[realPos.x, realPos.y];
            i++;
        }

        return agentSensors;
    }

    public Vector2Int GetRealPos(Vector2Int agentPos, Vector2Int sensorPos, Grid grid)
    {
        Vector2Int realPos = agentPos + sensorPos;
        if (realPos.x >= grid.width)
        {
            realPos.x = 0;
        }
        else if (realPos.x < 0)
        {
            realPos.x = grid.width - 1;
        }

        if (realPos.y >= grid.height)
        {
            realPos.y = 0;
        }
        else if (realPos.y < 0)
        {
            realPos.y = grid.height - 1;
        }

        return realPos;
    }

}
