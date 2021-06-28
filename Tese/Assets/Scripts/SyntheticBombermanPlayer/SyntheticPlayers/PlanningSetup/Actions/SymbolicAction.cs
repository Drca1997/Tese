using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SymbolicAction : MonoBehaviour
{
    public enum Tile
    {
        Player, PlayerEnemy, AIEnemy, Walkable, Explodable, Unsurpassable, Bomb, Fire,
        PlayerNBomb, PlayerEnemyNBomb, AIEnemyNBomb,
        FireNExplodable, FireNPlayer, FireNPlayerEnemy, FireNAIEnemy, FireNBomb,
        FireNBombNPlayer, FireNBombNPlayerEnemy, FireNBombNAIEnemy
    }

    public enum Action
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        PlantBomb,
        DoNothing
    }

    private PlanningSyntheticPlayer agent;
    private int rawAction; //integer that represents the action 
    [SerializeField]
    private int cost; //cost of the action when performing A*
    private int[,] effect; //Effects of the action
    [SerializeField]
    private SymbolicAction oppositeAction; //opposite action

    public abstract void Init(PlanningSyntheticPlayer agent);
        

    //Update the effect grid
    public void UpdateEffectGrid(int[,] newGrid)
    {
        Effect = SyntheticPlayerUtils.deepCopyWorld(newGrid);
    }

    public string DebugWorld()
    {
        string result = null;

        for (int i = Effect.GetLength(1) - 1; i >= 0; i--)
        {
            result += "\n";
            for (int j = 0; j < Effect.GetLength(0); j++)
            {
                result += Effect[j, i] + "|";
            }
        }
        return result;
    }
    public abstract bool IsPossible(int[,] grid);
    public abstract bool CheckPreconditions(int [,] grid);
    public abstract void Simulate();
    public abstract void Revert();

    public int RawAction { get => rawAction; set => rawAction = value; }
    public int[,] Effect { get => effect; set => effect = value; }
    public int Cost { get => cost; set => cost = value; }
    public SymbolicAction OppositeAction { get => oppositeAction; }
    public PlanningSyntheticPlayer Agent { get => agent; set => agent = value; }
}
