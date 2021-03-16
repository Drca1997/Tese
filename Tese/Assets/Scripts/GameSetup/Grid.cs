using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{

    private int width;
    private int height;
    private int[,] array;
    private TextMesh[,] debugTextArray;
    private Vector3 origin;
    private float cellSize;

    public Grid(int width, int height, Vector3 origin, float cellSize, bool aleatorio)
    {
        this.width = width;
        this.height = height;
        this.origin = origin;
        this.cellSize = cellSize;
        array = new int[width, height];
        debugTextArray = new TextMesh[width, height];
        GridSetup(aleatorio);
    }

    private void GridSetup(bool aleatorio)
    {
        if (aleatorio)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if ((i == 0 && j == 0) || (i == 0 && j == width - 1) ||
                        (i == height - 1 && j == 0) || (i == width - 1 && j == height - 1))
                    {
                        array[i, j] = 0;
                        
                    }

                    else
                    {
                        array[i, j] = Random.Range(1, 4);
                    }
                }
            }
        }

        else
        {
            int[,] tempArray = new int[,] { { 0, 1,1,2,2,1,1,0}, { 1,2,1,1,1,2,2,1}, {2,3,3,2,2,3,3,2}, {1,1,2,1,2,2,1,1},
                {1,3,3,2,2,3,3,1 }, { 2,3,1,1,1,2,3,2}, {1,2,2,2,1,2,2,1 }, {0,1,1,2,2,1,1,0 } };
            array = tempArray;
        }
    }
    public void DebugPrintGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.Log("X: " + x.ToString() + "\tY: " + y.ToString() + "\tValor: " + array[x, y]);
            }
        }
    }

    public void DisplayGrid(bool debugText)
    {
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                if (debugText)
                    debugTextArray[x, y] = Utils.CreateText(Array[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(CellSize, CellSize) * 0.5f, new Vector3(0.25f, 0.25f), Color.white, Mathf.FloorToInt(10 * cellSize), TextAnchor.MiddleCenter, TextAlignment.Center);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

            }
        }
        Debug.DrawLine(GetWorldPosition(0, Height), GetWorldPosition(Width, Height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(Width, Height), GetWorldPosition(Width, 0), Color.white, 100f);


    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * CellSize + origin;
    }

    /*
    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition.x - origin.x) / cellSize);
        y = Mathf.FloorToInt((worldPosition.y - origin.y) / cellSize);
    }*/

   

    public int Width
    {
        get => width;
        set => width = value;
    }

    public int Height
    {
        get => height;
        set => height = value;
    }

    public int[,] Array
    {
        get => array;
        set => array = value;
    }
    public Vector3 Origin
    {
        get => origin;
        set => origin = value;
    }

    public float CellSize
    {
        get => cellSize;
        set => cellSize = value;
    }


}
