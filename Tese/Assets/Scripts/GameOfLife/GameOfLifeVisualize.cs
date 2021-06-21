using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//IVisualize for the Game of Life with one agent type game scenario with representation for:
//LifeAgent Agents with regard to their internal state
public class GameOfLifeVisualize : MonoBehaviour, IVisualize
{
    public void VisualizeGrid(Grid grid)
    {
        //Going through every position on the agentGrid to update the respective one on the objectGrid
        for (int i = 0; i < grid.agentGrid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.agentGrid.GetLength(1); j++)
            {
                //Depending on the internal state of the LifeAgent, it will be represented black ("alive") or white ("dead")
                grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = (grid.agentGrid[i, j][0].states[1] == 1) ? Color.black : Color.white;
            }
        }
    }

    public string ReturnSet()
    {
        return "Game of Life v1";
    }
}
