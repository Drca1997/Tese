using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLifeSetup : MonoBehaviour, ISetup
{
    public int width = 20;
    public int height = 10;
    public float cellSize = 10f;
    [Range(0, 100)]
    public int randomFillPercetn = 20;
    public Grid SetupGrid(System.Random prng)
    {
        Agent[,] agentGrid = new LifeAgent[width, height];
        GameObject[,] objectGrid = new GameObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                agentGrid[x, y] = new LifeAgent(new List<int> { 0, (prng.Next(0, 100) < randomFillPercetn) ? 1 : 0 }, x, y);
                //CreateText(agentGrid[i, j].typeName, GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) / 2);
                objectGrid[x, y] = Utils.CreateSquare((agentGrid[x, y].states[1] == 1) ? Color.black : Color.white, agentGrid[x, y].typeName, Utils.GetWorldPosition(x, y, cellSize) + new Vector3(cellSize, cellSize) / 2, cellSize);
            }
        }
        Grid grid = new Grid(width, height, cellSize, agentGrid, objectGrid);
        return grid;
    }
}
