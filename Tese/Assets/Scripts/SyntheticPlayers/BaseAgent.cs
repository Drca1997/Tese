using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAgent: MonoBehaviour
{
    protected Grid grid;
    protected int x;
    protected int y;
    protected bool plantedbomb;

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
}
