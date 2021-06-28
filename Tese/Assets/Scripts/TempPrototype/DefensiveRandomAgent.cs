using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveRandomAgent : BaseAgent, IDecisionRequester
{
    private bool onDanger = false;
    public void GetWorld(TempGrid grid, int x, int y)
    {
        this.Grid = grid;
        this.x = x;
        this.y = y;
    }

    public int RequestDecision()
    {
        int action = Random.Range(0, 6);
        List<int[]> dangerTiles = CalculateDanger(onDanger, x, y);
        if (onDanger) {
            if (dangerTiles.Count == 0){
                while (!Utils.IsValidAction(Grid, this, action))
                {
                    action = Random.Range(0, 6);
                }
            }
            else
            {
                MoveOutOfDanger(dangerTiles);
            }
            
        }
        else
        {
            while (!Utils.IsValidAction(Grid, this, action))
            {
                action = Random.Range(0, 6);
            }
        }
       
        Debug.Log(gameObject.name + " " + Utils.ActionToString(action));
        return action;
    }

    private List<int[]> CalculateDanger(bool onDanger, int x, int y)
    {
        List<Bomb> bombs = GameObject.FindGameObjectWithTag("GameController").GetComponent<TempGameHandler>().getBombs();
        List<int[]> affectedTiles = new List<int[]>();
        foreach (Bomb bomba in bombs)
        {
            affectedTiles = bomba.CheckBombRadius();
        }
        foreach (int [] tile in affectedTiles)
        {
            if (tile[0] == x && tile[1] == y)
            {
                onDanger = true;
                
            }
            
        }
        if (affectedTiles.Count == 0)
        {
            onDanger = false;
        }
        return affectedTiles;
    }

    private int MoveOutOfDanger(List<int[]> dangerTiles)
    {
        List<int> possibleActions = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            if (Utils.IsValidAction(Grid, this, i))
            {
                possibleActions.Add(i);
            }
        }
        foreach (int action in possibleActions)
        {
            bool possibleDanger = true;
            int possibleX = x;
            int possibleY = y;
            switch (action)
            {
                case 0:
                    possibleY += 1;
                    break;
                case 1:
                    possibleY -= 1;
                    break;
                case 2:
                    possibleX -= 1;
                    break;
                case 3:
                    possibleX += 1;
                    break;
            } 
            if (CalculateDanger(possibleDanger, possibleX, possibleY).Count == 0)
            {
                Debug.Log("OOF, POR POUCO");
                return action;
            }
        }

        int randomAction = Random.Range(0, 6);
        while (!Utils.IsValidAction(Grid, this, randomAction))
        {
            randomAction = Random.Range(0, 6);
        }

        return randomAction;
    }
}
