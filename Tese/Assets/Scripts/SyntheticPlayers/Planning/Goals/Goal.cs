using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Goal: MonoBehaviour
{
    private int[,] gameWorld;
    [SerializeField]
    private PlanningAgent agent;
    [SerializeField]
    private int priority;
    private List<int[]> targetTiles;
    private int[] refTile;

    public int Priority { get => priority; set => priority = value; }
    public List<int[]> TargetTiles { get => targetTiles; set => targetTiles = value; }
    protected PlanningAgent Agent { get => agent; set => agent = value; }
    protected int[,] GameWorld { get => gameWorld; set => gameWorld = value; }
    public int[] RefTile { get => refTile; set => refTile = value; }
    public abstract void Init();

    public abstract bool IsPossible();

    public abstract double Heuristic(ActionStateGraphNode state, ActionStateGraphNode goal);

    public abstract bool IsObjective(ActionStateGraphNode node);

    public abstract int[,] GetGoalGrid(int[,] currentGrid, int index, PlanningAgent agent);
   

}
