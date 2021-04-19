using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SyntheticPlayerUtils
{
    public enum Tile
    {
        Player, PlayerEnemy, AIEnemy, Walkable, Explodable, Unsurpassable, Bomb, Fire,
        PlayerNBomb, PlayerEnemyNBomb, AIEnemyNBomb,
        FireNExplodable, FireNPlayer, FireNPlayerEnemy, FireNAIEnemy, FireNBomb,
        FireNBombNPlayer, FireNBombNPlayerEnemy, FireNBombNAIEnemy,
        Unknown
    }

    public static IEnumerable GridIterator(int[,] grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                yield return new int[2] { i, j };
            }
        }
    }

    public static bool IsValidAction(int [,] grid, SyntheticBombermanPlayer agent, int action)
    {
        switch (action)
        {
            case 0: //move up
                if (agent.position.y + 1 < grid.GetLength(1)) // se esta dentro dos limites
                {
                    Debug.Log("trying UP");
                    if (IsTileWalkable(grid, agent.position.x, agent.position.y + 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 1: //move down
                if (agent.position.y - 1 >= 0) // se esta dentro dos limites
                {
                    Debug.Log("trying DOWN");
                    if (IsTileWalkable(grid, agent.position.x, agent.position.y - 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 2: //move left
                if (agent.position.x - 1 >= 0) // se esta dentro dos limites
                {
                    Debug.Log("trying LEFT");
                    if (IsTileWalkable(grid, agent.position.x - 1, agent.position.y)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 3: //move right
                if (agent.position.x + 1 < grid.GetLength(0)) // se esta dentro dos limites
                {
                    Debug.Log("trying RIGHT");
                    if (IsTileWalkable(grid, agent.position.x + 1, agent.position.y)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 4: //plant bomb
                if (!agent.PlantedBomb)
                {
                    agent.PlantedBomb = true;
                    return true;
                }

                return false;

            case 5: //do nothing
                return true;
            default:
                break;
        }
        return true;
    }

    public static bool IsTileWalkable(int [,] grid, int x, int y)
    {
        Debug.Log(x + "," + y);
        Debug.Log(grid[x,y]);
        Debug.Log((int)Tile.Walkable);
        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            if (grid[x, y] == (int)Tile.Walkable)
            {
                Debug.Log("true");
                return true;
            }
        }
        Debug.Log("false");
        return false;
    }
}
