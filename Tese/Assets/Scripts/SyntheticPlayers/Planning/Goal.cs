using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Goal: MonoBehaviour
{
    private int[,] gameWorld;
    private BaseAgent agent;
    private int priority;
    private List<int[]> targetTiles;

    public int Priority { get => priority; set => priority = value; }
    public List<int[]> TargetTiles { get => targetTiles; set => targetTiles = value; }
    protected BaseAgent Agent { get => agent; set => agent = value; }
    protected int[,] GameWorld { get => gameWorld; set => gameWorld = value; }

}
