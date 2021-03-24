using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeSafeGoal : Goal
{

    public BeSafeGoal(int [,] world, BaseAgent agent)
    {
        this.GameWorld = world;
        this.Agent = agent;
        this.Priority = 1;
        this.TargetTiles = null;
    }
}
