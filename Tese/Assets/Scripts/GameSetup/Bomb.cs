using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb
{
    int countdown;
    int x, y;
    TempGrid grid;
    private BaseAgent agent;

    public Bomb(TempGrid grid, int x, int y, ref BaseAgent agent)
    {
        countdown = 3;
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.agent = agent;
    }

    public List<int[]> CheckBombRadius()
    {
        List<int[]> affectedTiles = new List<int[]>();
        checkNorth(affectedTiles, x, y);
        checkSouth(affectedTiles, x, y);
        checkWest(affectedTiles, x, y);
        checkEast(affectedTiles, x, y);

        return affectedTiles;
    }


    private bool IsTileAffected(int x, int y)
    {
        if (grid.Array[x, y] != 1 && grid.Array[x, y] != 3)
        {
            return true;
        }
        return false;
    }
    private void checkNorth(List<int[]> affectedTiles, int x, int y)
    {
        if (y + 1 < grid.Array.GetLength(1))
        {
            if (IsTileAffected(x, y + 1))
            {
                affectedTiles.Add(new int[] { x, y + 1 });
            }
            //se (x,y+2) nao está fora do mapa, e (x, y+1) nao é algo que tapou o radio da explosao
            if (y + 2 < grid.Array.GetLength(1) && grid.Array[x, y + 1] != 2 && grid.Array[x, y + 1] != 3)
            {
                if (IsTileAffected(x, y + 2))
                {
                    affectedTiles.Add(new int[] { x, y + 2 });
                }
            }
        }
    }

    private void checkSouth(List<int[]> affectedTiles, int x, int y)
    {
        if (y - 1 >= 0)
        {
            if (IsTileAffected(x, y - 1))
            {
                affectedTiles.Add(new int[] { x, y - 1 });
            }
            //se (x,y-2) nao está fora do mapa, e (x, y-1) nao é algo que tapou o radio da explosao
            if (y - 2 >= 0 && grid.Array[x, y - 1] != 2 && grid.Array[x, y - 1] != 3)
            {
                if (IsTileAffected(x, y - 2))
                {
                    affectedTiles.Add(new int[] { x, y - 2 });
                }
            }
        }
    }

    private void checkWest(List<int[]> affectedTiles, int x, int y)
    {
        if (x - 1 >= 0)
        {
            if (IsTileAffected(x - 1, y))
            {
                affectedTiles.Add(new int[] { x - 1, y });
            }
            //se (x-2,y) nao está fora do mapa, e (x-1, y) nao é algo que tapou o radio da explosao
            if (x - 2 >= 0 && grid.Array[x - 1, y] != 2 && grid.Array[x - 1, y] != 3)
            {
                if (IsTileAffected(x - 2, y))
                {
                    affectedTiles.Add(new int[] { x - 2, y });
                }
            }
        }
    }

   
    private void checkEast(List<int[]> affectedTiles, int x, int y)
    {
        if (x + 1 < grid.Array.GetLength(0))
        {
            if (IsTileAffected(x + 1, y))
            {
                affectedTiles.Add(new int[] { x + 1, y });
            }
            //se (x+2,y) nao está fora do mapa, e (x+1, y) nao é algo que tapou o radio da explosao
            if (x + 2 < grid.Array.GetLength(0) && grid.Array[x + 1, y] != 2 && grid.Array[x + 1, y] != 3)
            {
                if (IsTileAffected(x + 2, y))
                {
                    affectedTiles.Add(new int[] { x + 2, y });
                }
            }
        }
    }

    public int Countdown
    {
        get => countdown;
        set => countdown = value;
    }

    public BaseAgent Agent
    {
        get => agent;
        set => agent = value;
    }
    public int X
    {
        get => x;
        set => x = value;
    }
    public int Y
    {
        get => y;
        set => y = value;
    }
}
