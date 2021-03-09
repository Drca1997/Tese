using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bagingus : MonoBehaviour
{
    public int width = 20;
    public int height = 10;
    public float cellSize = 10f;

    public string seed;
    public bool useRandomSeed;
    private System.Random prng;
    [Range(0, 100)]
    public int randomFillPercetn;

    private Grid grid;

    private void Start()
    {
        if (useRandomSeed)
        {
            seed = System.DateTime.Now.ToString();
        }
        prng = new System.Random(seed.GetHashCode());

        Agent[,] agentGrid = GenerateLifeGrid(width, height);
        grid = new Grid(width, height, cellSize, agentGrid);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            UpdateAgents();
            grid.UpdateObjectGrid();
        }
    }

    private Agent[,] GenerateLifeGrid(int width, int height)
    {
        Agent[,] gridArray = new LifeAgent[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y] = new LifeAgent((prng.Next(0, 100) < randomFillPercetn) ? 1 : 0, x, y);
            }
        }

        return gridArray;
    }

    private void UpdateAgents()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Agent a = grid.agentGrid[x, y];
                Agent[] agentSensors = GetSensors(a);
                a.UpdateAgent(agentSensors);
            }
        }
    }

    private Agent[] GetSensors(Agent agent)
    {
        Vector2Int pos = agent.position;
        int numberSensors = agent.relative_sensors.Length + agent.constant_sensors.Length;

        Agent[] agentSensors = new Agent[numberSensors];
        int i = 0;

        foreach(Vector2Int sensorPos in agent.relative_sensors)
        {
            Vector2Int realPos = GetRealPos(agent.position, sensorPos);
            agentSensors[i] = grid.agentGrid[realPos.x, realPos.y];
            i++;
        }

        foreach (Vector2Int sensorPos in agent.constant_sensors)
        {
            Vector2Int realPos = GetRealPos(agent.position, sensorPos);
            agentSensors[i] = grid.agentGrid[realPos.x, realPos.y];
            i++;
        }

        return agentSensors;
    }

    private Vector2Int GetRealPos(Vector2Int agentPos, Vector2Int sensorPos)
    {
        Vector2Int realPos = agentPos + sensorPos;
        if (realPos.x >= width)
        {
            realPos.x = 0;
        }
        else if (realPos.x < 0)
        {
            realPos.x = width - 1;
        }

        if (realPos.y >= height)
        {
            realPos.y = 0;
        }
        else if (realPos.y < 0)
        {
            realPos.y = height - 1;
        }

        return realPos;
    }
}