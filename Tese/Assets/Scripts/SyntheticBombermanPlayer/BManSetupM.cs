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
        //0-nada
        //1-weak
        //2-strong
        //3-p
        //4-b
        //5-random
        //6-ml
        //7-idle

        int[,] setup_grid = {
               { 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 } ,
               { 2 , 6 , 0 , 0 , 1 , 0 , 1 , 1 , 1 , 0 , 0 , 1 , 0 , 5 , 2 } ,
               { 2 , 0 , 2 , 1 , 2 , 1 , 2 , 0 , 2 , 1 , 2 , 1 , 2 , 0 , 2 } ,
               { 2 , 1 , 1 , 0 , 1 , 1 , 0 , 0 , 0 , 1 , 1 , 0 , 1 , 0 , 2 } ,
               { 2 , 1 , 2 , 1 , 2 , 1 , 2 , 1 , 2 , 0 , 2 , 1 , 2 , 0 , 2 } ,
               { 2 , 1 , 1 , 0 , 1 , 0 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 1 , 2 } ,
               { 2 , 0 , 2 , 0 , 2 , 0 , 2 , 0 , 2 , 1 , 2 , 0 , 2 , 1 , 2 } ,
               { 2 , 1 , 1 , 1 , 0 , 1 , 0 , 0 , 1 , 1 , 1 , 1 , 1 , 0 , 2 } ,
               { 2 , 1 , 2 , 0 , 2 , 0 , 2 , 1 , 2 , 1 , 2 , 0 , 2 , 1 , 2 } ,
               { 2 , 0 , 1 , 1 , 1 , 1 , 1 , 0 , 1 , 1 , 0 , 1 , 1 , 0 , 2 } ,
               { 2 , 0 , 2 , 0 , 2 , 1 , 2 , 0 , 2 , 1 , 2 , 1 , 2 , 0 , 2 } ,
               { 2 , 7 , 0 , 0 , 1 , 1 , 0 , 1 , 1 , 1 , 0 , 1 , 0 , 5 , 2 } ,
               { 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 , 2 } ,
             };

        List<GameAgent>[,] agentGrid = new List<GameAgent>[setup_grid.GetLength(1), setup_grid.GetLength(0)];
        for (int x = 0; x < setup_grid.GetLength(1); x++)
        {
            for (int y = 0; y < setup_grid.GetLength(0); y++)
            {
                agentGrid[x, y] = new List<GameAgent> { };
                switch(setup_grid[y,x])
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
                        agentGrid[x, y].Add(new ABomberman(new List<int> { }, x, y, GetComponent<IUpdate>()));
                        break;
                    case 5:
                        agentGrid[x, y].Add(new RandomSyntheticPlayer(new List<int> { }, x, y, GetComponent<IUpdate>()));
                        break;
                    case 6:
                        agentGrid[x, y].Add(new MLSyntheticPlayer(new List<int> { }, x, y, GetComponent<IUpdate>(), gameObject.GetComponent<MLAgent>()));
                        break;
                    case 7:
                        agentGrid[x, y].Add(new IdleSyntheticPlayer(new List<int> { }, x, y, GetComponent<IUpdate>()));
                        break;

                }
            }
        }

        //Grid constructed with the agentGrid
        Grid grid = new Grid(setup_grid.GetLength(1), setup_grid.GetLength(0),
            10, agentGrid, new string[] { "Malaquias_Bomberman", "PlayerBomberman", "Agent_Bomberman", "Walkable", "Agent_Weak_Wall", "Agent_Strong_Wall", "Agent_Bomb", "Agent_Fire"});

        return grid;
    }
}