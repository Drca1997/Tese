using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAgent: MonoBehaviour
{
    private TempGrid grid;
    protected int x;
    protected int y;
    protected bool plantedbomb;
    private bool isAlive = true;

    public bool PlantedBomb
    {
        get => plantedbomb;
        set => plantedbomb = value;
    }
    public int X
    {
        get => x;
        set => x = value;
    }
    public int Y
    {
        get => y;
        set => y = value;
    }
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public TempGrid Grid { get => grid; set => grid = value; }
}
