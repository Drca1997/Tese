using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains both the agentGrid and objectGrid
//At each moment represents the current time step of the simulation 
public class Grid 
{
    //Width of both agentGrid and objectGrid
    public int width;
    //Height of both agentGrid and objectGrid
    public int height;
    //Cellsize of both agentGrid and objectGrid
    public float cellSize;

    //A matrix of lists of Agents
    //Each position of the matrix may contain multiple Agents
    public List<GameAgent>[,] agentGrid;

    //An Array containting all the agentTypes on the grid
    //Can be used to map a List<Agent>[,] into a List<int>[,]
    public string[] agentTypes;

    //A matrix of GameObjects
    //Can be understood as the visual representation of the agentGrid
    public GameObject[,] objectGrid;

    //GameObject parent to all the GameObjects contained in objectGrid
    public GameObject container;

    //Bollean that indicates if the objectGrid may be updated with new information from the agentGrid
    public bool updated;

    //Bollean that indicates if the simulation has finished 
    public bool simOver;


    //Receives int (width), int (height), float (cellSize), List<Agent>[,] (agentGrid), string[] (agentTypes)
    //Grid constructor
    //Initiates the objectGrid
    public Grid (int width, int height, float cellSize, List<GameAgent>[,] agentGrid, string[] agentTypes)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.agentGrid = agentGrid;
        this.agentTypes = agentTypes;
        this.objectGrid = new GameObject[width, height];
        this.updated = false;
        this.simOver = false;

        container = new GameObject("GridContainer");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //the GameObjects are inititated with their default state (color white and name "Empty")
                objectGrid[x, y] = Utils.CreateSquare(Color.white, "Empty", GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) / 2, cellSize);
                //Setting the new GameObject as the child of container
                objectGrid[x, y].transform.SetParent(container.transform);
            }
        }
    }

    //Receives in (x) and int (y)
    //Returns Vector3
    //Maps and returns the game world position equivalent to the given x and y on the grid
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + container.transform.position;
    }

    //Receives Vector3 (worldPosition)
    //Returns Vector2
    //Maps and returns the x and y on the grid equivalent to the given game world position
    public Vector2 GetXY(Vector3 worldPosition)
    {
        return new Vector2(Mathf.FloorToInt((worldPosition- container.transform.position).x/cellSize), Mathf.FloorToInt((worldPosition - container.transform.position).y / cellSize));
    }

    public List<int>[,] ConvertAgentGrid()
    {
        List<int>[,] convertedGrid = new List<int>[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                convertedGrid[x, y] = new List<int> { };
                foreach (GameAgent a in agentGrid[x, y])
                {
                    convertedGrid[x, y].Add(GetAgentTypeInt(a.typeName));
                }
            }
        }
        return convertedGrid;
    }

    public int GetAgentTypeInt(string type)
    {
        return Array.IndexOf(agentTypes, type);
    }

    public void deleteContainer()
    {
        foreach (Transform child in container.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject.Destroy(container);
    }
}
