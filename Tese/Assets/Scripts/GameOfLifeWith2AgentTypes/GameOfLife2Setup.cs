using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ISetup Interface for the Game of Life with two agent types game scenario with:
//a random distribution of LifeAgentAlive Agents
//a number of RandomMoveAgent Agents
//a number of PlayerMovementAgent Agents
public class GameOfLife2Setup : MonoBehaviour, ISetup
{
    //width of the simulation grid, by default 20 units
    public int width = 20;
    //height of the simulation grid, by default 10 units
    public int height = 10;
    //cell size of the simulation grid, by default 10f units
    public float cellSize = 10f;

    //number of RandomMoveAgent Agents on the grid, by default 1
    public int numberRandomMovingAgents = 1;
    //number of PlayerMovementAgent Agents on the grid, by default 1
    public int numberPlayerAgents = 1;

    //percentage of cells with a LifeAgentAlive, by default 20%
    [Range(0, 100)]
    public int randomFillPercetn = 20;


    //Receives a System.Random as a parameter that may be used for randomization
    //Returns a Grid object, setted up for the start of the simulation
    //This function is responsible for the creation of the Grid object, the dimensions of the grid, and the initial distribution of agents in the agentGrid
    public Grid SetupGrid(System.Random prng)
    {

        //Creation and initialization of the agentGrid, with randomFillPercetn of positions with a LifeAgentAlive
        //Other positions are initialized with a LifeAgentDead
        List<Agent>[,] agentGrid = new List<Agent>[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                agentGrid[x, y] = new List<Agent> { };

                if (prng.Next(0, 100) < randomFillPercetn)
                {
                    agentGrid[x, y].Add(new LifeAgentAlive(new List<int> { 0 }, x, y));
                }
                else
                {
                    agentGrid[x, y].Add(new LifeAgentDead(new List<int> { 0 }, x, y));
                }
                
            }
        }

        //numberRandomMovingAgents RandomMoveAgent Agents positioned on a random location within the grid
        int randx;
        int randy;
        int index=0;
        while (index < numberRandomMovingAgents)
        {
            randx = prng.Next(0, width);
            randy = prng.Next(0, height);
            //RandomMoveAgent are positioned on points of the agentGrid without other RandomMoveAgent
            if (Utils.AgentListContinesType(agentGrid[randx, randy], "Random_Move_Agent") == null)
            {
                Agent moveAgent = new RandomMoveAgent(new List<int> { prng.Next(0, 4), 10, 5 }, randx, randy);
                agentGrid[randx, randy].Add(moveAgent);
                index++;
            }
        }

        //numberPlayerAgents PlayerMovementAgent Agents positioned on a random location within the grid
        index = 0;
        while (index < numberPlayerAgents)
        {
            randx = prng.Next(0, width);
            randy = prng.Next(0, height);
            //PlayerMovementAgent are positioned on points of the agentGrid without other PlayerMovementAgent
            if (Utils.AgentListContinesType(agentGrid[randx, randy], "Player_Movement_Agent") == null)
            {
                AgentPlayer playerAgent = new PlayerMovementAgent(new List<int> { prng.Next(0, 4) }, randx, randy, this);
                agentGrid[randx, randy].Add(playerAgent);
                index++;
            }
        }


        //Grid constructed with the agentGrid
        Grid grid = new Grid(width, height, cellSize, agentGrid);

        return grid;
    }
}
