using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLifeUpdate : MonoBehaviour, IUpdate
{
    public Grid UpdateGrid(Grid grid, System.Random prng)
    {
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Agent a = grid.agentGrid[x, y];
                grid = a.UpdateAgent(grid, 0);
            }
        }

        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Agent a = grid.agentGrid[x, y];
                grid = a.UpdateAgent(grid, 1);
            }
        }
        return grid;
    }
}
