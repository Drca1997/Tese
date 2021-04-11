using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    
    public const int beSafeGoalPriority = 1;
    public const int attackEnemyGoalPriority = 2;
    public const int gridWidth = 8;
    public const double explodibleBlockCost = 5.0;

    public static int[,] deepCopyWorld(int[,] world)
    {
        int[,] deepCopy = new int[world.GetLength(0), world.GetLength(1)];
        for (int i = 0; i < world.GetLength(0); i++)
        {
            for (int j = 0; j < world.GetLength(1); j++)
            {
                deepCopy[i, j] = world[i, j];
            }
        }
        return deepCopy;
    }
    public static IEnumerable GridIterator(int[,] grid)
    {
        for (int i=0; i < grid.GetLength(0); i++)
        {
            for (int j=0; j < grid.GetLength(1); j++)
            {
               yield return new int[2] { i, j };
            }
        }
    }


    public static int [] GetTileFromIndex(int index, int width)
    {
        int x = index / width;
        int y = index % width;
        return new int[2] {x, y };
    }

    public static List<int> GetNeighbouringTilesIndexes(int [,] grid, int index)
    {
        int[] tempIndexes = new int[4];
        tempIndexes[0] = index - grid.GetLength(0);
        tempIndexes[1] = index - 1;
        tempIndexes[2] = index + 1;
        tempIndexes[3] = index + grid.GetLength(0);
        List<int> validIndexes = new List<int>();
        if (tempIndexes[0] >= 0) //north neighbour
        {
            validIndexes.Add(tempIndexes[0]);
        }
        if (index % grid.GetLength(0) != 0) //EastNeighbour-> 1ºelem de cada linha é multiplo da largura do grid
        {
            validIndexes.Add(tempIndexes[1]);
        }
        if (tempIndexes[2] % grid.GetLength(0) != 0)//West Neighbour-> ultimo elem de cada linha nao pode ser multiplo da largura do grid
        {
            validIndexes.Add(tempIndexes[2]);
        }
        if (tempIndexes[3] < grid.GetLength(0) * grid.GetLength(1)) //south neighbour
        {
            validIndexes.Add(tempIndexes[3]);
        }
        return validIndexes;

    }

    public static List<int[]> GetAdjacentTiles(int[,] grid, int[] tile)
    {
        List<int[]> adjacentTiles = new List<int[]>();
        if (tile[1] + 1 < grid.GetLength(1))
        {
            adjacentTiles.Add(new int[2] { tile[0], tile[1] + 1 });

            if (tile[1] + 2 < grid.GetLength(1))
            {
                adjacentTiles.Add(new int[2] { tile[0], tile[1] + 2 });
            }
        }
        if (tile[1] - 1 >= 0)
        {
            adjacentTiles.Add(new int[2] { tile[0], tile[1] - 1 });
            if (tile[1] - 2 >= 0)
            {
                adjacentTiles.Add(new int[2] { tile[0], tile[1] - 2 });
            }

        }
        if (tile[0] + 1 < grid.GetLength(0))
        {
            adjacentTiles.Add(new int[2] { tile[0] + 1, tile[1] });
            if (tile[0] + 2 < grid.GetLength(0))
            {
                adjacentTiles.Add(new int[2] { tile[0] + 2, tile[1] });
            }
        }
        if (tile[0] - 1 >= 0)
        {
            adjacentTiles.Add(new int[2] { tile[0] - 1, tile[1] });
            if (tile[0] - 2 >= 0)
            {
                adjacentTiles.Add(new int[2] { tile[0] - 2, tile[1] });
            }
        }
        return adjacentTiles;
    }

    public static Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static TextMesh CreateText(string text, Transform parent, Vector3 position, Vector3 scale, Color color, int fontsize, TextAnchor anchor, TextAlignment alignment)
    {
        GameObject obj = new GameObject("Text", typeof(TextMesh));

        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        TextMesh label = obj.GetComponent<TextMesh>();
        label.text = text;
        label.color = color;
        label.fontSize = fontsize;
        label.anchor = anchor;
        label.alignment = alignment;
        return label;
    }

    public static GameObject CreateSpriteRenderer(Vector3 worldPosition, Sprite sprite, float cellSize)
    {
        GameObject obj = new GameObject();
        obj.transform.position = worldPosition + new Vector3(cellSize, cellSize) * 0.5f;
        obj.AddComponent<SpriteRenderer>();
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        return obj;
    }

    public static bool IsTileWalkable(Grid grid, int x, int y) {
        if (x >= 0 && x < grid.Width && y >= 0 && y < grid.Height)
        {
            if (grid.Array[x, y] == 1 || grid.Array[x, y] == 4)
            {
                return true;
            }

            
        }
        return false;
    }

    public static string ActionToString(int action)
    {
        switch (action)
        {
            case 0:
                return "moveu-se para cima";
            case 1:
                return "moveu-se para baixo";
            case 2:
                return "moveu-se para a esquerda";
            case 3:
                return "moveu-se para a direita";
            case 4:
                return "PLANTOU BOMBA";
            case 5:
                return "permaneceu no mesmo sítio";
               
        }
        return null;
    }

    public static bool IsValidAction(Grid grid, BaseAgent agent, int action)
    {
        switch (action)
        {
            case 0: //move up
                if (agent.Y + 1 < grid.Array.GetLength(1)) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X, agent.Y + 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 1: //move down
                if (agent.Y - 1 >= 0) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X, agent.Y - 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 2: //move left
                if (agent.X - 1 >= 0) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X - 1, agent.Y)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 3: //move right
                if (agent.X + 1 < grid.Array.GetLength(0)) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X + 1, agent.Y)) //Se é walkable
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


    public static bool IsTileSafe(int[,] grid, int [] tile)
    {
       
        if (grid[tile[0], tile[1]] == 4) // se esta uma bomba na tile
        {
            return false;
        }
        if(!IsNorthTilesSafe(grid, tile))
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

        if (tile[1] + 1 < grid.GetLength(1) && grid[tile[0], tile[1] + 1] == 4) //se bomba em (x, y+1)
        {
            return false;
        }

        if (tile[1] + 2 < grid.GetLength(1) && grid[tile[0], tile[1] + 2] == 4) //se bomba em (x, y+2)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (grid[tile[0], tile[1] + 1] != 2 && grid[tile[0], tile[1] + 1] != 3)
            {
                return false;
            }

        }
        return true;
    }
    
    private static bool IsSouthTilesSafe(int[,] grid, int[] tile)
    {
        if (tile[1] -1 >=0 &&grid[tile[0], tile[1] - 1] == 4)//se bomba em (x, y-1)
        {
            return false;
        }
        if (tile[1] - 2 >= 0 &&grid[tile[0], tile[1] - 2] == 4)//se bomba em (x, y-2)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (grid[tile[0], tile[1] - 1] != 2 && grid[tile[0], tile[1] - 1] != 3)
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsWestTilesSafe(int[,] grid,int[] tile)
    {
        if (tile[0] - 1 >= 0 && grid[tile[0] - 1, tile[1]] == 4) // se bomba em (x-1, y)
        {
            return false;
        }
        if (tile[0] - 2 >= 0 &&grid[tile[0] - 2, tile[1]] == 4) // se bomba em (x-2, y)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (grid[tile[0] - 1, tile[1]] != 2 && grid[tile[0] - 1, tile[1]] != 3)
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsEastTilesSafe(int[,] grid, int[] tile)
    {
        if (tile[0] + 1 < grid.GetLength(0) && grid[tile[0] + 1, tile[1]] == 4) // se bomba em (x+1, y)
        {
            return false;
        }
        if (tile[0] + 2 < grid.GetLength(0) && grid[tile[0] + 2, tile[1]] == 4) // se bomba em (x+2, y)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (grid[tile[0] + 1, tile[1]] != 2 && grid[tile[0] + 1, tile[1]] != 3)
            {
                return false;
            }
        }
        return true;
    }

    public static bool [,] dangerMap(int[,] grid)
    {
        bool[,] dangerMap = new bool[grid.GetLength(0), grid.GetLength(1)];
        for (int i=0; i < grid.GetLength(0); i++)
        {
            for (int j=0; j < grid.GetLength(1); j++)
            {
                if (!IsTileSafe(grid, new int[2] { i, j}))
                {
                    dangerMap[i, j] = false;
                }
                else
                {
                    dangerMap[i, j] = true;
                }
            }
        }
        return dangerMap;
    }

    public static List<int[]> dangerTiles(bool[,] dangerMap, bool reverse) //reverse indica se devolve dangerTiles(false) ou safeTiles(true)
    {
        List<int[]> dangerTiles = new List<int[]>();
        for (int i=0; i < dangerMap.GetLength(0); i++)
        {
            for (int j=0; j < dangerMap.GetLength(1); j++)
            {
                if (reverse)
                {
                    if (!dangerMap[i, j])
                    {
                        dangerTiles.Add(new int[2] { i, j });
                    }

                }
                else
                {
                    if (dangerMap[i, j])
                    {
                        dangerTiles.Add(new int[2] { i, j });
                    }
                }
               
            }
        }
        return dangerTiles;
    }
    

    /**
    public static List<int[]> GetTilesInBounds(Grid grid, int x, int y)
    {
        List<int[]> tiles = new List<int[]>();
        if (y + 1 < grid.Array.GetLength(1))
        {
            if (.IsTileAffected(x, y + 1))
            {
                tiles.Add(new int[] { x, y + 1 });
            }
            //se (x,y+2) nao está fora do mapa, e (x, y+1) nao é algo que tapou o radio da explosao
            if (y + 2 < grid.Array.GetLength(1) && grid.Array[x, y + 1] != 2 && grid.Array[x, y + 1] != 3)
            {
                if (IsTileAffected(x, y + 2))
                {
                    tTiles.Add(new int[] { x, y + 2 });
                }
            }
        }
        return tiles;
    }*/
}
