using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemyGoal : AttackEntityGoal
{

    public override bool IsPossible()
    {
        Debug.Log("Verificando se é possível atacar inimigo");
       
        if (RefTile != null && PlanningAgent.GridArray[RefTile[0], RefTile[1]] != (int)Tile.AIEnemy) //Update RefTile
        {
            Debug.Log("UPDATE REFTILE");
            RefTile = null;
        }
        else if (RefTile != null && (PlanningAgent.GridArray[RefTile[0], RefTile[1]]  == (int)Tile.AIEnemy || PlanningAgent.GridArray[RefTile[0], RefTile[1]] == (int)Tile.FireNAIEnemy || PlanningAgent.GridArray[RefTile[0], RefTile[1]] == (int)Tile.FireNBombNAIEnemy)) //Caso RefTile ainda referencie a posição do inimigo
        {
            Debug.Log("REFTILE REFERENCIA INIMIGO AINDA");
            Debug.Log("REFTILE: " + RefTile[0] +  ", " + RefTile[1]);
            this.TargetTiles = SyntheticPlayerUtils.GetAdjacentTiles(PlanningAgent.GridArray, RefTile);
            foreach (int[] tile in TargetTiles)
            {
                if (PlanningAgent.GridArray[tile[0], tile[1]] == (int)Tile.Bomb)
                {
                    return false;
                }
            }
        }

        if (RefTile == null) //Caso RefTile seja nula, Procurar por inimigos
        {
            Debug.Log("Procurando por inimigos...");
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
        Debug.Log("Possível atacar inimigo em " + RefTile[0] + ", " + RefTile[1]);
        return true;
    }
}
