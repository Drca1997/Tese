using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ISetup Interface for the Game of Life with two agent types game scenario with:
//a "glider" structure setted up from the beginning
public class GameOfLife2SetupGlider : MonoBehaviour, ISetup
{
    //cell size of the simulation grid, by default 10f units
    public float cellSize = 10f;


    //Receives a System.Random as a parameter that may be used for randomization
    //Returns a Grid object, setted up for the start of the simulation
    //This function is responsible for the creation of the Grid object, the dimensions of the grid, and the initial distribution of agents in the agentGrid
    public Grid SetupGrid(System.Random prng, int width, int height)
    {

        //Creation of a setup matrix with the structure of a "glider"
        //each cell in the setup matrix indicates if a LifeAgentAlive Agent should be placed at the same position in the agentGrid
        int[,] setupGrid = new int[width, height];
        if (width > 5 & height > 5)
        {
            setupGrid[1, height - 1] = 1;
            setupGrid[2, height - 2] = 1;
            setupGrid[0, height - 3] = 1;
            setupGrid[1, height - 3] = 1;
            setupGrid[2, height - 3] = 1;
        }

        //Creation and initialization of the agentGrid acording to the setupGrid
        //positions with value 1 in the setupGrid have a LifeAgentAlive on the agentGrid
        //positions with value 0 in the setupGrid have a LifeAgentDead on the agentGrid
        List<GameAgent>[,] agentGrid = new List<GameAgent>[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                agentGrid[x, y] = new List<GameAgent> { };
                if (setupGrid[x, y] == 1)
                {
                    agentGrid[x, y].Add(new LifeAgentAlive(new List<int> { 0 }, x, y));
                }
                else
                {
                    agentGrid[x, y].Add(new LifeAgentDead(new List<int> { 0 }, x, y));
                }
            }
        }

        //Grid constructed with the agentGrid
        Grid grid = new Grid(width, height, cellSize, agentGrid, new string[] { "Life_Agent_Dead", "Life_Agent_Alive", "Player_Movement_Agent", "Random_Move_Agent" });
        return grid;
    }

    public string ReturnSet()
    {
        return "Game of Life v2";
    }

    public string ReturnName()
    {
        return "Glider";
    }

}