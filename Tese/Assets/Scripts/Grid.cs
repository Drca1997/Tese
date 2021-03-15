using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid 
{
    public int width;
    public int height;
    public float cellSize;
    public Agent[,] agentGrid;
    public GameObject[,] objectGrid;

    public Grid (int width, int height, float cellSize, Agent[,] agentGrid, GameObject[,] objectGrid)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.agentGrid = agentGrid;
        this.objectGrid = objectGrid;
    }
}
