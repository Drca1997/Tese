using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBlockGoal : AttackEntityGoal
{
    public override bool IsPossible()
    {
        Debug.Log("Verificando se é possível explodir bloco");
        if (RefTile != null && PlanningAgent.GridArray[RefTile[0], RefTile[1]] != (int)Tile.Explodable) //Update RefTile
        {
            RefTile = null;
        }
        else if (RefTile != null && (PlanningAgent.GridArray[RefTile[0], RefTile[1]] == (int)Tile.Explodable || PlanningAgent.GridArray[RefTile[0], RefTile[1]] == (int)Tile.FireNExplodable)) //Caso RefTile ainda referencie a posição do inimigo
        {
            this.TargetTiles = SyntheticPlayerUtils.GetAdjacentTiles(PlanningAgent.GridArray, RefTile);
            foreach (int[] tile in TargetTiles)
            {
                if (PlanningAgent.GridArray[tile[0], tile[1]] == (int)Tile.Bomb)
                {
                    return false;
                }
            }
        }

        if (RefTile == null) //Caso RefTile seja nula, Procurar por blocos
        {
            Debug.Log("Procurando por blocos para explodir");
            GetEntityPos();
            if (RefTile != null)
            {

                this.TargetTiles = SyntheticPlayerUtils.GetAdjacentTiles(PlanningAgent.GridArray, RefTile);
                foreach (int[] tile in TargetTiles)
                {
                    if (PlanningAgent.GridArray[tile[0], tile[1]] == (int)Tile.Bomb)
                    {
                        return false;
                    }
                }
            }
            else
            {

                return false;
            }


        }
        Debug.Log("Possível explodir bloco em " + RefTile[0] + ", " + RefTile[1]);
        return true;
    }
}

   
