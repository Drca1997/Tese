using System.Collections.Generic;
using UnityEngine;

//ISetup Interface for the Bomberman game scenario with:
//a random distribution of Weak Wall Agents,
//a Bomberman Player Agent
public class BManSetup2 : MonoBehaviour, ISetup
{
    

    
    public Grid SetupGrid(System.Random prng, int width, int height)
    {
        //0-nada
        //1-weak
        //2-strong
        //3-p
        //4-b

        //int[,] setup_grid = {
        //       { 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 } ,
        //       { 2 , 4 , 0 , 0 , 1 , 0 , 1 , 1 , 1 , 0 , 0 , 1 , 0 , 4 , 2 } ,
        //       { 2 , 0 , 2 , 1 , 2 , 1 , 2 , 0 , 2 , 1 , 2 , 1 , 2 , 0 , 2 } ,
        //       { 2 , 1 , 1 , 0 , 1 , 1 , 0 , 0 , 0 , 1 , 1 , 0 , 1 , 0 , 2 } ,
        //       { 2 , 1 , 2 , 1 , 2 , 1 , 2 , 1 , 2 , 0 , 2 , 1 , 2 , 0 , 2 } ,
        //       { 2 , 1 , 1 , 0 , 1 , 0 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 2 } ,
        //       { 2 , 0 , 2 , 0 , 2 , 0 , 2 , 0 , 2 , 1 , 2 , 0 , 2 , 1 , 2 } ,
        //       { 2 , 1 , 1 , 1 , 0 , 1 , 0 , 0 , 1 , 1 , 1 , 1 , 1 , 0 , 2 } ,
        //       { 2 , 1 , 2 , 0 , 2 , 0 , 2 , 1 , 2 , 1 , 2 , 0 , 2 , 1 , 2 } ,
        //       { 2 , 0 , 1 , 1 , 1 , 1 , 1 , 0 , 1 , 1 , 0 , 1 , 1 , 0 , 2 } ,
        //       { 2 , 0 , 2 , 0 , 2 , 1 , 2 , 0 , 2 , 1 , 2 , 1 , 2 , 0 , 2 } ,
        //       { 2 , 3 , 0 , 0 , 1 , 1 , 0 , 1 , 1 , 1 , 0 , 1 , 0 , 4 , 2 } ,
        //       { 1 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 } ,
        //     };

        //List<GameAgent>[,] agentGrid = new List<GameAgent>[setup_grid.GetLength(1), setup_grid.GetLength(0)];
        //for (int x = 0; x < setup_grid.GetLength(1); x++)
        //{
        //    int gridY = setup_grid.GetLength(0) - 1;
        //    for (int y = 0; y < setup_grid.GetLength(0); y++)
        //    {
        //        agentGrid[x, y] = new List<GameAgent> { };
        //        switch (setup_grid[gridY, x])
        //        {
        //            case 1:
        //                agentGrid[x, y].Add(new AWeakWall(new List<int> { }, x, y));
        //                break;
        //            case 2:
        //                agentGrid[x, y].Add(new AStrongWall(new List<int> { }, x, y));
        //                break;
        //            case 3:
        //                agentGrid[x, y].Add(new PBomberman(new List<int> { }, x, y, this, GetComponent<IUpdate>()));
        //                break;
        //            case 4:
        //                agentGrid[x, y].Add(new ABomberman(new List<int> { }, x, y, GetComponent<IUpdate>()));
        //                break;
        //        }
        //        gridY--;
        //    }
        //}

        IGenerateMap MapGenerator = GetComponent<IGenerateMap>();

        List<int>[,] setup_grid = MapGenerator.GenerateMap(prng, width, height);


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
                            agentGrid[x, y].Add(new PBomberman(new List<int> { }, x, y, this, GetComponent<IUpdate>()));
                            break;
                        case 4:
                        case 5:
                        case 6:
                            agentGrid[x, y].Add(new ABomberman(new List<int> { }, x, y, GetComponent<IUpdate>()));
                            break;
                    }
                }
            }
        }


        //Grid constructed with the agentGrid
        Grid grid = new Grid(setup_grid.GetLength(0), setup_grid.GetLength(1), 10, agentGrid, new string[] { "Agent_Weak_Wall", "Agent_Strong_Wall", "Player_Bomberman", "Agent_Bomberman", "Agent_Bomb", "Agent_Fire"});

        return grid;
    }

    public string ReturnSet()
    {
        return "Bomberman";
    }

    public string ReturnName()
    {
        return "Classic";
    }
}