using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningSyntheticPlayer : SyntheticBombermanPlayer
{

    private Goal[] allGoals;
    private SymbolicAction [] allActions;
    private List<SymbolicAction> currentPlan;
    private Goal currentGoal;
    //Simulated Position of the agent when planning
    private int simulatedX;
    private int simulatedY;
    private bool firstTime;

    public int SimulatedX { get => simulatedX; set => simulatedX = value; }
    public int SimulatedY { get => simulatedY; set => simulatedY = value; }

    public PlanningSyntheticPlayer(List<int> states, int x, int y, IUpdate updateInterface, Goal [] allGoals, SymbolicAction [] allActions) : base(states, x, y, updateInterface)
    {

        this.allActions = allActions;
        this.allGoals = allGoals;
        System.Array.Sort(allGoals, delegate (Goal x, Goal y) { return x.Priority.CompareTo(y.Priority); });
        Debug.Log("Possible Actions: " + allActions.Length);
        Debug.Log("Possible Goals: " + allGoals.Length);
        currentGoal = null;
        SimulatedX = x;
        SimulatedY = y;
        firstTime = true;
        currentPlan = new List<SymbolicAction>();

        foreach (Goal goal in allGoals)
        {
            goal.GetPlayerRef(this);
        }

        
        Debug.Log("POSITION: " + position.x + "," + position.y);
    }

    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        //podes usar g.agentGrid para obter a matriz de List<Agent>
        //g.ConvertAgentGrid() devolve-te uma List<int>[,] correspondente
        //tem em conta que algumas das listas podem estar vazias ou ter multiplos elementos, visto que há sitios na grelha sem ou com multiplos agentes
        //Os inteiros correspondem aos indices que os tipos de agente ocupam em g.agentTypes - podes modificá-los na interface de setup, na criação da nova Grid

        //Para mover o agente e criar uma bomba podes consultar o meu codigo em PBomberman.cs na função Logic

        //Debug.Log("UPDATING AGENT in " + position.x + ", " + position.y);
        
        HasBomb();
        gridArray = ConvertGrid(g);
        Debug.Log(SyntheticPlayerUtils.DebugGrid(gridArray));
        if (firstTime)
        {
            foreach (SymbolicAction action in allActions) //o grid do agente é null ate que nao se faça UpdateAgente pela 1ª vez. Entao acçoes nao podem ser iniciadas no construtor do agente 
            {
                action.Init(this);
                //Debug.Log(action.Agent);
            }
            firstTime = false;
        }
        ProcessAction(g, TakeAction());

    }

    public override int TakeAction()
    {
        Debug.Log("REQUESTING DECISION");
        int nextAction;
        if (HasPlan()) //verificar se plano ainda é exequivel e se sim, efetuar proxima açao
        {
            if (currentGoal != null)
            {
                Debug.Log("Verificando plano existente");

                if (currentGoal.IsPossible() && currentPlan[0].IsPossible(gridArray))
                {
                    Debug.Log("Possível de avançar para a próxima ação do plano");
                    nextAction = AdvancePlan();

                }
                else
                {
                    Debug.Log("Impossível seguir com plano em frente. A gerar um novo plano...");
                    currentPlan = Plan(); //Gera Novo Plan
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
            Debug.Log("Ação a executar este turno: " + SyntheticPlayerUtils.ActionToString(nextAction));
            currentPlan.Remove(currentPlan[0]);
            return nextAction;
        }
        Debug.Log("Impossível seguir com plano à frente");
        return -1;
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

    private List<SymbolicAction> Plan()
    {
        Debug.Log("A gerar novo plano...");
        currentGoal = null;
        Goal goal = GetNextGoal();
        currentGoal = goal;
        if (goal == null)
        {
           
            return null;
        }

        List<GraphNode> pathfindingNodes = NavGraph.GetPath(GridArray, position.x, position.y, goal);
        if (pathfindingNodes == null)
        {
            return null;
        }
        int goalNodeIndex = GetGoalNodeIndex(goal, pathfindingNodes);
        int[] simTile = SyntheticPlayerUtils.GetTileFromIndex(goalNodeIndex, GridArray.GetLength(0));
        SimulatedX = simTile[0];
        simulatedY = simTile[1];
        Debug.Log("Goal Tile:" + simTile[0] + ", " + simTile[1]);
        WorldNode goalNode = new WorldNode(0, this, goal.GetGoalGrid(GridArray, goalNodeIndex, this)); //cria nó com base no estado objetivo
        WorldNode currentNode = new WorldNode(1, this, GridArray); //cria nó com base no estado atual do jogo
        

        List<SymbolicAction> newPlan = AStar.AStarForPlanning(this, goalNode, currentNode, allActions, goal); //aquigoalNode e currentNode trocam-se pois é procura regressiva
        DebugPlan(newPlan);

        return newPlan;
    }


    private Goal GetNextGoal()
    {
        foreach (Goal goal in allGoals)
        {
            if (goal.IsPossible())
            {
                Debug.Log("Objetivo encontrado: " + goal.GetType());

                return goal;
            }
        }
        Debug.Log("Nenhum objetivo disponível");

        return null;
    }

    private int GetGoalNodeIndex(Goal goal, List<GraphNode> list)
    {
        if (goal.GetType() == typeof(AttackEnemyGoal))
        {
            if (list.Count >= 2)
            {
                return list[list.Count - 2].Index; //obtém ultimo elemento, que é o nó objetivo
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
        simulatedX = position.x;
        simulatedY = position.y;
    }


    private void DebugPlan(List<SymbolicAction> plan)
    {
        if (plan != null)
        {
            string result = null;
            foreach (SymbolicAction action in plan)
            {
                if (action != null)
                {
                    result += SyntheticPlayerUtils.ActionToString(action.RawAction) + "->";
                }

            }
            Debug.Log("PLANO:\n" + result);
        }
        else
        {
            Debug.Log("PLANO VAZIO!");
        }

    }

    //usa para algo que querias que o agente faça ao ser removido da grid
    //de momento meti codigo para o agente avisar a interface de update que "morreu", para se saber quando a simulação deve ser parada
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {
        //na função AgentCall a interface vai lidar com decrementar a sua variável que indica o numero de jogadores
        updateInterface.AgentCall(this, g, prng);
    }

}

