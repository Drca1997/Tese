using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class SyntheticPlayerUtils
{
    public enum Tile
    {
        Player, PlayerEnemy, AIEnemy, Walkable, Explodable, Unsurpassable, Bomb, Fire,
        PlayerNBomb, PlayerEnemyNBomb, AIEnemyNBomb,
        FireNExplodable, FireNPlayer, FireNPlayerEnemy, FireNAIEnemy, FireNBomb,
        FireNBombNPlayer, FireNBombNPlayerEnemy, FireNBombNAIEnemy
    }
    public static int[] bombTiles = new int[6] { (int)Tile.Bomb, (int)Tile.AIEnemyNBomb, (int)Tile.PlayerEnemy, (int)Tile.FireNBomb, (int)Tile.FireNBombNAIEnemy, (int)Tile.FireNBombNPlayerEnemy };
    public static int[] coverTiles = new int[3] { (int)Tile.Explodable, (int)Tile.Unsurpassable, (int)Tile.FireNExplodable };
    public enum Action
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        PlantBomb,
        DoNothing
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
                    if (IsTileWalkable(grid, agent.position.x, agent.position.y + 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 1: //move down
                if (agent.position.y - 1 >= 0) // se esta dentro dos limites
                {
                    if (IsTileWalkable(grid, agent.position.x, agent.position.y - 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 2: //move left
                if (agent.position.x - 1 >= 0) // se esta dentro dos limites
                {
                    if (IsTileWalkable(grid, agent.position.x - 1, agent.position.y)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 3: //move right
                if (agent.position.x + 1 < grid.GetLength(0)) // se esta dentro dos limites
                {
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
        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            if (grid[x, y] == (int)Tile.Walkable)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsTileSafe(int[,] grid, int[] tile)
    {
        
        if (bombTiles.Contains(grid[tile[0], tile[1]])) // se esta uma bomba na tile
        {
            return false;
        }
        if (!IsNorthTilesSafe(grid, tile))
        {
            return false;
        }
        if (!IsSouthTilesSafe(grid, tile))
        {
            return false;
        }
        if (!IsEastTilesSafe(grid, tile))
        {
            return false;
        }
        if (!IsWestTilesSafe(grid, tile))
        {
            return false;
        }
        return true;
    }

    private static bool IsNorthTilesSafe(int[,] grid, int[] tile)
    {

        if (tile[1] + 1 < grid.GetLength(1) && bombTiles.Contains(grid[tile[0], tile[1] + 1])) //se bomba em (x, y+1)
        {
            return false;
        }

        if (tile[1] + 2 < grid.GetLength(1) && bombTiles.Contains(grid[tile[0], tile[1] + 2])) //se bomba em (x, y+2)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (!coverTiles.Contains(grid[tile[0], tile[1] + 1]))
            {
                return false;
            }

        }
        return true;
    }

    private static bool IsSouthTilesSafe(int[,] grid, int[] tile)
    {
        if (tile[1] - 1 >= 0 && bombTiles.Contains(grid[tile[0], tile[1] - 1]))//se bomba em (x, y-1)
        {
            return false;
        }
        if (tile[1] - 2 >= 0 && bombTiles.Contains(grid[tile[0], tile[1] - 2]))//se bomba em (x, y-2)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (!coverTiles.Contains(grid[tile[0], tile[1] - 1]))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsWestTilesSafe(int[,] grid, int[] tile)
    {
        if (tile[0] - 1 >= 0 && bombTiles.Contains(grid[tile[0] - 1, tile[1]])) // se bomba em (x-1, y)
        {
            return false;
        }
        if (tile[0] - 2 >= 0 && bombTiles.Contains(grid[tile[0] - 2, tile[1]])) // se bomba em (x-2, y)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (!coverTiles.Contains(grid[tile[0] - 1, tile[1]]))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsEastTilesSafe(int[,] grid, int[] tile)
    {

        if (tile[0] + 1 < grid.GetLength(0) && bombTiles.Contains(grid[tile[0] + 1, tile[1]])) // se bomba em (x+1, y)
        {
            return false;
        }
        if (tile[0] + 2 < grid.GetLength(0) && bombTiles.Contains(grid[tile[0] + 2, tile[1]])) // se bomba em (x+2, y)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (!coverTiles.Contains(grid[tile[0] + 1, tile[1]]))
            {
                return false;
            }
        }
        return true;
    }
    public static int GetDistToClosestEnemy(int[,] gridArray, int[] agentPos, int[] tilesWithAgents)
    {
        int closestDist = int.MaxValue;

        foreach (int[] tile in SyntheticPlayerUtils.GridIterator(gridArray))
        {
            if (tilesWithAgents.Contains(gridArray[tile[0], tile[1]]))
            {
                int dist = SyntheticPlayerUtils.CalculateManhattanDistance(agentPos, tile);
                if (dist < closestDist)
                {
                    closestDist = dist;
                }
            }
        }
        return closestDist;
    }
    public static int CalculateManhattanDistance(int[] start, int[] end)
    {
        return Mathf.Abs(end[0] - start[0]) + Mathf.Abs(end[1] - start[1]);
    }

    public static string ActionToString(int action)
    {
        switch (action)
        {
            case 0:
                return "ACTION: MOVE UP";
            case 1:
                return "ACTION: MOVE DOWN";
            case 2:
                return "ACTION: MOVE LEFT";
            case 3:
                return "ACTION: MOVE RIGHT";
            case 4:
                return "ACTION: PLANT BOMB";
            case 5:
                return "ACTION: DO NOTHING";

        }
        return null;
    }
}
