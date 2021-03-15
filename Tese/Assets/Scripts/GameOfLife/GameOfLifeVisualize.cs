using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLifeVisualize : MonoBehaviour, IVisualize
{
    public void VisualizeGrid(Grid grid)
    {
        for (int i = 0; i < grid.agentGrid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.agentGrid.GetLength(1); j++)
            {
                grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = (grid.agentGrid[i, j].states[1] == 1) ? Color.black : Color.white;
            }
        }
    }
}
