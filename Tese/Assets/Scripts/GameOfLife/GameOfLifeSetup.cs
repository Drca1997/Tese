using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ISetup Interface for the Game of Life with one agent type game scenario with:
//a random distribution of "alive" LifeAgents
public class GameOfLifeSetup : MonoBehaviour, ISetup
{
    //width of the simulation grid, by default 20 units
    public int width = 20;
    //height of the simulation grid, by default 10 units
    public int height = 10;
    //cell size of the simulation grid, by default 10f units
    public float cellSize = 10f;

    //percentage of cells with a LifeAgent with a states[1] component with value 1 (meaning its "alive"), by default 20%
    [Range(0, 100)]
    public int randomFillPercetn = 20;

    //Receives a System.Random as a parameter that may be used for randomization
    //Returns a Grid object, setted up for the start of the simulation
    //This function is responsible for the creation of the Grid object, the dimensions of the grid, and the initial distribution of agents in the agentGrid
    public Grid SetupGrid(System.Random prng)
    {
        //Creation and initialization of the agentGrid, with randomFillPercetn of positions with a LifeAgent with a states[1] component with value 1 (meaning its "alive")
        //Other positions are initialized with a LifeAgent with a states[1] component with value 0 (meaning its "dead")
        List<GameAgent>[,] agentGrid = new List<GameAgent>[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                agentGrid[x, y] = new List<GameAgent> { };
                agentGrid[x, y].Add(new LifeAgent(new List<int> { 0, (prng.Next(0, 100) < randomFillPercetn) ? 1 : 0 }, x, y));
            }
        }

        //Grid constructed with the agentGrid
        Grid grid = new Grid(width, height, cellSize, agentGrid, new string[] { "Live_Agent" });

        return grid;
    }
}
