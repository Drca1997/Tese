using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySetup: MonoBehaviour, ISetup
{
    //cell size of the simulation grid, by default 10f units
    public float cellSize = 10f;
    public Grid SetupGrid(System.Random prng, int width, int height)
    {
        List<GameAgent>[,] agentGrid = new List<GameAgent>[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                agentGrid[x, y] = new List<GameAgent> { };
            }
        }


        //Grid constructed with the agentGrid
        Grid grid = new Grid(width, height, cellSize, agentGrid, new string[] {});

        return grid;
    }

    public string ReturnSet()
    {
        return "Empty";
    }

    public string ReturnName()
    {
        return "Empty";
    }

}
