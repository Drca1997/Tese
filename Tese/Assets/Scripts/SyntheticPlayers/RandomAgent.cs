using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAgent : BaseAgent, IDecisionRequester
{

    public void GetWorld(Grid grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    bool IsValidAction(int action) {
        switch (action)
        {
            case 0: //move up
                if (y + 1 < grid.Array.GetLength(1)) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, x, y+1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 1: //move down
                if (y - 1 >= 0) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, x, y - 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 2: //move left
                if (x - 1 >= 0) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, x-1, y)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 3: //move right
                if (x + 1 < grid.Array.GetLength(0)) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, x+1, y)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 4: //plant bomb
                if (!plantedbomb)
                {
                    plantedbomb = true;
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

    public int RequestDecision()
    {
        int action = Random.Range(0, 6);
        while (!IsValidAction(action))
        {
            action = Random.Range(0, 6);
        }
        Debug.Log(gameObject.name + " " + Utils.ActionToString(action));
        return action;
    }


}
