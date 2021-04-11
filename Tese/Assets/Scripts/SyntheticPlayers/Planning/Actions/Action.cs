using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action: MonoBehaviour 
{
    [SerializeField]
    PlanningAgent agent;
    [SerializeField]
    private int rawAction;
    [SerializeField]
    private int cost;
    private int[,] effect;
    //[SerializeField]
    //private string[] preconditionsKeys;
    [SerializeField]
    private Action oppositeAction;
    //private Dictionary<string, bool> preconditions;

    private void Start()
    {
        
        //preconditions = new Dictionary<string, bool>();
        Effect = Utils.deepCopyWorld(Agent.Grid.Array);
        /*
        foreach (string precondition in preconditionsKeys)
        {
            ProcessPrecondition(precondition);
        }*/
    }
    /*
    private void ProcessPrecondition(string precondition)
    {
        switch (precondition)
        {
            case "CanMoveLeft":
                    preconditions.Add(precondition, Utils.IsTileWalkable(Agent.Grid, Agent.X - 1, Agent.Y));
                break;
            case "CanMoveRight":
                preconditions.Add(precondition, Utils.IsTileWalkable(Agent.Grid, Agent.X + 1, Agent.Y));
                break;
            case "CanMoveUp":
                preconditions.Add(precondition, Utils.IsTileWalkable(Agent.Grid, Agent.X, Agent.Y+1));
                break;
            case "CanMoveDown":
                preconditions.Add(precondition, Utils.IsTileWalkable(Agent.Grid, Agent.X, Agent.Y-1));
                break;
            case "CanPlantBomb":

                preconditions.Add(precondition, !Agent.PlantedBomb);
                break;
        }
    }*/

    public void UpdateEffectGrid(int[,] newGrid)
    {
        Effect = Utils.deepCopyWorld(newGrid);
    }

    public abstract bool CheckPreconditions();
    public abstract void Simulate();
    public abstract void Revert();
    //public abstract void Execute(Grid grid, BaseAgent agent);
    public int RawAction { get => rawAction; set => rawAction = value; }
    public int[,] Effect { get => effect; set => effect = value; }
    public int Cost { get => cost; set => cost = value; }
    public PlanningAgent Agent { get => agent; set => agent = value; }
    public Action OppositeAction { get => oppositeAction;}
}
