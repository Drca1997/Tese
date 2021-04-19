using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningAgent : BaseAgent, IDecisionRequester
{
 
    private Action [] possibleActions;
    private Goal[] possibleGoals;
    private List<Action> currentPlan;
    private Goal currentGoal;
    private int simulatedX;
    private int simulatedY;
    private bool simulatedPlantedBomb;
    
    private void Start()
    {
        possibleActions = gameObject.GetComponents<Action>();
        possibleGoals = gameObject.GetComponents<Goal>();
        System.Array.Sort(possibleGoals, delegate (Goal x, Goal y) { return x.Priority.CompareTo(y.Priority); });
        Debug.Log("Possible Actions: " + possibleActions.Length);
        Debug.Log("Possible Goals: " + possibleGoals.Length);
        currentGoal = null;
        SimulatedX = X;
        SimulatedY = Y;
        SimulatedPlantedBomb = PlantedBomb;
        currentPlan = new List<Action>();

        foreach (Goal goal in possibleGoals)
        {
            goal.Init();
        }
    }

    public void GetWorld(TempGrid grid, int x, int y)
    {
        this.Grid = grid;
        this.x = x;
        this.y = y;
    }

    public int RequestDecision()
    {
        
       
        Debug.Log("REQUESTING DECISION");
        int nextAction;
        if (HasPlan()) //verificar se plano ainda � exequivel e se sim, efetuar proxima a�ao
        {
            if (currentGoal != null)
            {
                Debug.Log("Verificando plano existente");
               
                if (currentGoal.IsPossible() && currentPlan[0].IsPossible())
                {
                    Debug.Log("Poss�vel de avan�ar para a pr�xima a��o do plano");
                    nextAction = AdvancePlan();

                }
                else
                {
                    Debug.Log("Imposs�vel seguir com plano em frente. A gerar um novo plano...");
                    currentPlan = Plan(); //Gera Novo Plan
                    nextAction = AdvancePlan(); //Efetua Primeira A��o do Plano
                    ResetSim(); //reset agents' planning sim atributes 

                }
            }
            else
            {
                Debug.Log("Algo correu mal");
                nextAction = -1;
            }            
        }
        else
        {
            
            currentPlan = Plan(); //Gera Novo Plano
            nextAction = AdvancePlan(); //Efetua Primeira A��o do Plano
            ResetSim(); //reset agents' planning sim atributes 
        }
        return nextAction;
    }

    public int AdvancePlan()
    {
        if (currentPlan != null && currentPlan.Count > 0)
        {
            int nextAction = currentPlan[0].RawAction;
            Debug.Log("A��o a executar este turno: " + Utils.ActionToString(nextAction));
            currentPlan.Remove(currentPlan[0]);
            return nextAction;
        }
        Debug.Log("Imposs�vel seguir com plano � frente");
        return -1;
    }

    private List<Action> Plan()
    {
        Debug.Log("A gerar novo plano...");
        currentGoal = null;
        Goal goal = GetNextGoal();
        currentGoal = goal;
        if (goal == null)
        {
            return null;
        }
        
        List<GraphNode> pathfindingNodes= NavigationGraph.GetPath(Grid.Array, X, Y, goal);
        int goalNodeIndex = GetGoalNodeIndex(goal, pathfindingNodes);
       
        int [] simTile = Utils.GetTileFromIndex(goalNodeIndex,  Grid.Width);
        SimulatedX = simTile[0];
        simulatedY = simTile[1];

        ActionStateGraphNode goalNode = new ActionStateGraphNode(0, this, goal.GetGoalGrid(Grid.Array, goalNodeIndex, this)); //cria n� com base no estado objetivo
        ActionStateGraphNode currentNode = new ActionStateGraphNode(1, this, this.Grid.Array); //cria n� com base no estado atual do jogo
      
        List<Action> newPlan = AStar.AStarForPlanning(this, goalNode, currentNode, possibleActions, goal); //aquigoalNode e currentNode trocam-se pois � procura regressiva
        DebugPlan(newPlan);
       
        return newPlan;
    }

    private bool HasPlan()
    {
        if (currentPlan == null || currentPlan.Count == 0)
        {
            Debug.Log("Plano vazio");
            return false;
        }
        Debug.Log("Plano cont�m " + currentPlan.Count + " ac��es");
        return true;
    }

    private Goal GetNextGoal()
    {
        foreach (Goal goal in possibleGoals)
        {
            if (goal.IsPossible()) { 
                Debug.Log("Objetivo encontrado: " + goal.GetType());
               
                return goal; 
            }
        }
        Debug.Log("Nenhum objetivo dispon�vel");
      
        return null;
    }

    private int GetGoalNodeIndex(Goal goal, List<GraphNode> list)
    {
        if (goal.GetType() == typeof(AttackEnemyGoal))
        {
            if (list.Count >= 2)
            {
                return list[list.Count - 2].Index; //obt�m ultimo elemento, que � o n� objetivo
            }
            else
            {
                return list[0].Index;
            }
        }
        else if (goal.GetType() == typeof(BeSafeGoal))
        {
            return list[list.Count - 1].Index;
        }
        return -1;
    }
    private void ResetSim()
    {
        SimulatedPlantedBomb = plantedbomb;
        simulatedX = X;
        simulatedY = Y;
    }
    private void DebugPlan(List<Action> plan)
    {
        if (plan != null)
        {
            string result = null;
            foreach (Action action in plan)
            {
                if (action != null)
                {
                    result += Utils.ActionToString(action.RawAction) + "->";
                }
                    
            }
           Debug.Log("PLANO:\n" + result);
        }
        else
        {
            Debug.Log("PLANO VAZIO!");
        }
        
    }

    public Action [] PossibleActions { get => possibleActions; set => possibleActions = value; }
    public int SimulatedX { get => simulatedX; set => simulatedX = value; }
    public int SimulatedY { get => simulatedY; set => simulatedY = value; }
    public bool SimulatedPlantedBomb { get => simulatedPlantedBomb; set => simulatedPlantedBomb = value; }
}
