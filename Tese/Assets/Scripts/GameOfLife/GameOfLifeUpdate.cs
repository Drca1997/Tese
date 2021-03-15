using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLifeUpdate : MonoBehaviour, IUpdate
{
    public Grid UpdateGrid(Grid grid, System.Random prng)
    {
        List<Agent> randList = Utils.PutAgentsInList(grid.agentGrid);
        Utils.Shuffle<Agent>(randList, prng);
        foreach(Agent a in randList)
        {
            grid = a.UpdateAgent(grid, 0);
        }

        foreach (Agent a in randList)
        {
            grid = a.UpdateAgent(grid, 1);
        }
        return grid;
    }
}
