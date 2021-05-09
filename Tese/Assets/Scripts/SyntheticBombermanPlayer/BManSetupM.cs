using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ISetup Interface for the Bomberman game scenario with:
//a random distribution of Weak Wall Agents,
//a Bomberman Player Agent
public class BManSetupM : MonoBehaviour, ISetup
{


    public Grid SetupGrid(System.Random prng)
    {
        IGenerateMap MapGenerator = GetComponent<IGenerateMap>();

        List<int>[,] setup_grid = MapGenerator.GenerateMap(prng);


        List<GameAgent>[,] agentGrid = new List<GameAgent>[setup_grid.GetLength(0), setup_grid.GetLength(1)];
        for (int x = 0; x < setup_grid.GetLength(0); x++)
        {
            for (int y = 0; y < setup_grid.GetLength(1); y++)
            {
                agentGrid[x, y] = new List<GameAgent> { };
                foreach (int i in setup_grid[x, y])
                {
                    switch (i)
                    {
                        case 1:
                            agentGrid[x, y].Add(new AWeakWall(new List<int> { }, x, y));
                            break;
                        case 2:
                            agentGrid[x, y].Add(new AStrongWall(new List<int> { }, x, y));
                            break;
                        case 3:
                            agentGrid[x, y].Add(new IdleSyntheticPlayer(new List<int> { }, x, y, GetComponent<IUpdate>()));
                            break;
                        case 4:
                            agentGrid[x, y].Add(new MLSyntheticPlayer(new List<int> { }, x, y, GetComponent<IUpdate>(), gameObject.GetComponent<MLAgent>()));
                            break;
                        case 5:
                        case 6:
                            agentGrid[x, y].Add(new RandomSyntheticPlayer(new List<int> { }, x, y, GetComponent<IUpdate>()));
                            break;

                    }
                }
            }
        }
        //Grid constructed with the agentGrid
        Grid grid = new Grid(setup_grid.GetLength(0), setup_grid.GetLength(1),
            10, agentGrid, new string[] { "Malaquias_Bomberman", "PlayerBomberman", "Agent_Bomberman", "Walkable", "Agent_Weak_Wall", "Agent_Strong_Wall", "Agent_Bomb", "Agent_Fire"});

        return grid;
    }
}