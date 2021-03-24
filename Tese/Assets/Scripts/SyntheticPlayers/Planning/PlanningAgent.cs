using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningAgent : BaseAgent, IDecisionRequester
{
 
    private Action [] possibleActions;
    private Goal[] possibleGoals;
    private List<Action> currentPlan;
    private Goal currentGoal;
    
    private void Start()
    {
        possibleActions = gameObject.GetComponents<Action>();
        possibleGoals = gameObject.GetComponents<Goal>();
        System.Array.Sort(possibleGoals, delegate (Goal x, Goal y) { return x.Priority.CompareTo(y.Priority); });
        Debug.Log("Possible Actions: " + possibleActions.Length);
        Debug.Log("Possible Goals: " + possibleGoals.Length);
        currentPlan = null;
        currentGoal = null;
    }

    public void GetWorld(Grid grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public int RequestDecision()
    {
        if (HasPlan())
        {
            //verificar se plano ainda é exequivel e se sim, efetuar proxima açao

            return 0;
        }
        else
        {
            currentPlan = Plan();
            return currentPlan[0].RawAction;
        }
    }

    private List<Action> Plan()
    {
        GetNextGoal();
        
        return null;
    }

    private bool HasPlan()
    {
        if (currentPlan.Count == 0)
        {
            return false;
        }
        return true;
    }

    private Goal GetNextGoal()
    {
        foreach (Goal goal in possibleGoals)
        {
            //verificar se goal é possivel
            return goal;
        }
        return null;
    }

    public Action [] PossibleActions { get => possibleActions; set => possibleActions = value; }
}
