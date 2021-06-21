using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ISetup Interface for the Bomberman game scenario with:
//a random distribution of Weak Wall Agents,
//a Bomberman Player Agent
public class BManSetup : MonoBehaviour, ISetup
{
    //cell size of the simulation grid, by default 10f units
    public float cellSize = 10f;

    //percentage of cells with a Weak Wall Agent, by default 20%
    [Range(0, 100)]
    public int randomFillPercent = 20;

    //Receives a System.Random as a parameter that may be used for randomization
    //Returns a Grid object, setted up for the start of the simulation
    //This function is responsible for the creation of the Grid object, the dimensions of the grid, and the initial distribution of agents in the agentGrid
    public Grid SetupGrid(System.Random prng, int width, int height)
    {
        //Creation and initialization of the agentGrid, with randomFillPercetn of positions with a Weak Wall Agent
        List<GameAgent>[,] agentGrid = new List<GameAgent>[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                agentGrid[x, y] = new List<GameAgent> { };
                if (prng.Next(0, 100) < randomFillPercent)
                {
                    int agentType = prng.Next(0, 3);
                    switch (agentType)
                    {
                        case 0:
                            agentGrid[x, y].Add(new AStrongWall2(new List<int> { 100, 100 }, x, y));
                            break;
                        case 1:
                            agentGrid[x, y].Add(new AWeakWall2(new List<int> { 100, 100 }, x, y));
                            break;
                        case 2:
                            agentGrid[x, y].Add(new ABush(new List<int> { }, x, y));
                            break;

                    }
                    
                }
                else if (prng.Next(0, 100) < randomFillPercent)
                {
                    
                }
            }
        }

        //Grid constructed with the agentGrid
        Grid grid = new Grid(width, height, cellSize, agentGrid, new string[] { "Agent_Weak_Wall", "Agent_Strong_Wall", "Player_Bomberman", "Agent_Bomberman", "Agent_Bomb", "Agent_Fire" });

        return grid;
    }

    public string ReturnSet()
    {
        return "Bomberman";
    }

    public string ReturnName()
    {
        return "Random";
    }
}