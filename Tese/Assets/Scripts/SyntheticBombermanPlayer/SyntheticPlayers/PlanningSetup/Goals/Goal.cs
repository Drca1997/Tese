using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Goal: MonoBehaviour
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

    private PlanningSyntheticPlayer planningAgent;

    [SerializeField]
    private int priority;
    private List<int[]> targetTiles;
    private int[] refTile;

    public List<int[]> TargetTiles { get => targetTiles; set => targetTiles = value; }
    public int[] RefTile { get => refTile; set => refTile = value; }

    public PlanningSyntheticPlayer PlanningAgent { get => planningAgent; set => planningAgent = value; }
    public int Priority { get => priority; set => priority = value; }

    public void GetPlayerRef(PlanningSyntheticPlayer agent)
    {
        PlanningAgent = agent;
    }

    public abstract bool IsPossible();

    public abstract double Heuristic(WorldNode state, WorldNode goal);

    public abstract bool IsObjective(WorldNode node);

    public abstract int[,] GetGoalGrid(int[,] currentGrid, int index, PlanningSyntheticPlayer agent);
}
