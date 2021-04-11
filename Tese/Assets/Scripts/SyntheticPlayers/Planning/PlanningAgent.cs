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

    public void GetWorld(Grid grid, int x, int y)
    {
        this.Grid = grid;
        this.x = x;
        this.y = y;
    }

    public int RequestDecision()
    {
        
       
        Debug.Log("REQUESTING DECISION");
        int nextAction;
        if (HasPlan()) //verificar se plano ainda é exequivel e se sim, efetuar proxima açao
        {
            if (currentGoal != null)
            {
                Debug.Log("Verificando plano existente");
               
                if (currentGoal.IsPossible() && currentPlan[0].CheckPreconditions())
                {
                    Debug.Log("Possível de avançar para a próxima ação do plano");
                    nextAction = AdvancePlan();

                }
                else
                {
                    Debug.Log("Impossível seguir com plano em frente. A gerar um novo plano...");
                    currentPlan = Plan(); //Gera Novo Plano
                    DebugPlan(currentPlan);
                    nextAction = AdvancePlan(); //Efetua Primeira Ação do Plano
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
            DebugPlan(currentPlan);
            nextAction = AdvancePlan(); //Efetua Primeira Ação do Plano
            ResetSim(); //reset agents' planning sim atributes 
        }
        return nextAction;
    }

    public int AdvancePlan()
    {
        if (currentPlan != null && currentPlan.Count > 0)
        {
            int nextAction = currentPlan[0].RawAction;
            Debug.Log("Ação a executar este turno: " + Utils.ActionToString(nextAction));
            currentPlan.Remove(currentPlan[0]);
            if (currentPlan.Count > 0) { Debug.Log("Ação Seguinte: " + Utils.ActionToString(currentPlan[0].RawAction)); }
            return nextAction;
        }
        Debug.Log("Impossível seguir com plano à frente");
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
        int goalNodeIndex;
        if (pathfindingNodes.Count >= 2)
        {
            goalNodeIndex = pathfindingNodes[pathfindingNodes.Count - 2].Index; //obtém ultimo elemento, que é o nó objetivo
        }
        else{
            goalNodeIndex = pathfindingNodes[0].Index;
        }
        int [] simTile = Utils.GetTileFromIndex(goalNodeIndex,  Grid.Width);
        SimulatedX = simTile[0];
        simulatedY = simTile[1]; 
      

        ActionStateGraphNode goalNode = new ActionStateGraphNode(0, this, goal.GetGoalGrid(Grid.Array, goalNodeIndex, this)); //cria nó com base no estado objetivo
        string world = goalNode.DebugWorld();
        Debug.Log(world);
       
        ActionStateGraphNode currentNode = new ActionStateGraphNode(1, this, this.Grid.Array); //cria nó com base no estado atual do jogo
      
        List<Action> newPlan = AStar.AStarForPlanning(this, goalNode, currentNode, goal.IsObjective, possibleActions, goal.Heuristic); //aquigoalNode e currentNode trocam-se pois é procura regressiva
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
        Debug.Log("Plano contém " + currentPlan.Count + " acções");
        return true;
    }

    private Goal GetNextGoal()
    {
        Debug.Log("GETNEXTGOAL");
        foreach (Goal goal in possibleGoals)
        {
            //verificar se goal é possivel
           if (goal.GetType() == typeof(AttackEnemyGoal)){
                Debug.Log("IS ATTACKENEMYGOAL POSSIBLE");
               
            }
           else if (goal.GetType() == typeof(BeSafeGoal))
            {
                Debug.Log("IS BESAFEGOAL POSSIBLE");
              
            }
            if (goal.IsPossible()) { 
                Debug.Log("Objetivo encontrado");
               
                return goal; 
            }
        }
        Debug.Log("Nenhum objetivo disponível");
      
        return null;
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
            foreach (Action action in plan)
            {
                if (action != null)
                    Debug.Log(Utils.ActionToString(action.RawAction) + "->");
            }
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
