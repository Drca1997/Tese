using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningSyntheticPlayer : SyntheticBombermanPlayer
{

    private Goal[] allGoals; //list with all goals the agent can pursure
    private SymbolicAction [] allActions; //list of all actions the agent can execute
    private List<SymbolicAction> currentPlan; //the current plan (sequence of actions) to execute
    private Goal currentGoal; //current goal the agent is pursuing
    private int simulatedX; //Simulated X Coordinate of the agent's position when planning
    private int simulatedY; //Simulated Y Coordinate of the agent's position when planning

    private bool firstTime;

    public int SimulatedX { get => simulatedX; set => simulatedX = value; }
    public int SimulatedY { get => simulatedY; set => simulatedY = value; }

    /**
     * Constructor of an Agent Instance
     * List<int> states: an intrinsic attribute of a platform's game agent. This particular agent does not use this
     * int x: the x coordinate of the agent’s position in the grid 
     * int y: the y coordinate of the agent’s position in the grid
     * IUpdate updateInterface: reference to the platform’s update interface, responsible for the game loop update.
     * Goal [] allGoals: a vector of all the possible goals the agent can pursue during a game session
     * SymbolicAction [] allActions: a vector of all the possible actions the agent can perform during a game session
     */
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

    //Method responsible for updating the agent in its turn
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
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

   
    //The Decision Making Process of the Agent
    public override int TakeAction()
    {
        Debug.Log("REQUESTING DECISION");
        int nextAction;
        //If the agent has a plan, and there is not any higher-priorit goal than the current one, and the current goal is still possible to achieve, then
        if (HasPlan() && GetMoreImportantObjectives(currentGoal) == null && currentGoal != null && currentGoal.IsPossible())
        {
            nextAction = MoveCheck(currentPlan[0]); //check the safety of the action to be executed
        }
        else 
        {
            Replanning();
            if (currentPlan != null)
            {
                nextAction = MoveCheck(currentPlan[0]); //check the safety of the action to be executed
            }
            else
            {
                nextAction = (int)Action.DoNothing;
            }
            
        }
        //ReactionTime(); //implemented delay to record videos for the surveys

        return nextAction;
    }

    /**
     * Method to retrive higher-priority goals than the one provided as input 
     */
    private Goal GetMoreImportantObjectives(Goal currentgoal)
    {
        foreach (Goal goal in allGoals)
        {
            if (goal.Priority > currentGoal.Priority)
            {
                return goal;
            }
            else if (goal == currentgoal)
            {
                return null;
            }
        }
        return null;
    }

    /**
     * Advances the plan, executing its next action
     */
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

    //Checks if the agent currently has a plan
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

    //Plan() + ResetSim()
    private void Replanning()
    {
        Debug.Log("Impossível seguir com plano em frente. A gerar um novo plano...");
        currentPlan = Plan(); //Gera Novo Plan
        ResetSim(); //reset agents' planning sim atributes 
    }

    //Checks if action is possible and advances plan
    private int MoveCheck(SymbolicAction action)
    {

        if (action.IsPossible(gridArray))
        {
            return AdvancePlan(); //Efetua Primeira Ação do Plano
        }
        else
        {
            return (int)Action.DoNothing; //caso tenha plano mas a 1ª ação neste timestep não pode ser executada.
                                          //Por exemplo, quando quer perseguir um inimigo mas a primeira tile pelo caminho não é segura
        }
    }

    /**
     * Agent's Planning Process
     * */
    private List<SymbolicAction> Plan()
    {
        Debug.Log("A gerar novo plano...");
        currentGoal = null;
        Goal goal = GetNextGoal(); //First it gets a goal to pursue
        currentGoal = goal;
        if (goal == null) //if there is no possible goal to pursure, then return null
        {
           
            return null;
        }

        //A* Pathfinding in order to get the nearest goal tile
        List<GraphNode> pathfindingNodes = NavGraph.GetPath(GridArray, position.x, position.y, goal);
        if (pathfindingNodes == null)
        {
            return null;
        }
        int goalNodeIndex = GetGoalNodeIndex(goal, pathfindingNodes); //get index of nearest goal tile
        int[] simTile = SyntheticPlayerUtils.GetTileFromIndex(goalNodeIndex, GridArray.GetLength(0));
        
        //simulated position of the agent becomes equal to the goal tile's coordinates
        SimulatedX = simTile[0]; 
        simulatedY = simTile[1];
        //Debug.Log("Goal Tile:" + simTile[0] + ", " + simTile[1]);


        WorldNode goalNode = new WorldNode(0, this, goal.GetGoalGrid(GridArray, goalNodeIndex, this)); //creates search node with goal state
        WorldNode currentNode = new WorldNode(1, this, GridArray); //creates search node with current game state

        //A*for planning is invoked. Here goalNode is the starting node of the search and currentNode the goal node, 
        //since we are performing regressive search
        List<SymbolicAction> newPlan = AStar.AStarForPlanning(this, goalNode, currentNode, allActions, goal); 
        DebugPlan(newPlan);

        return newPlan;
    }

    /**
     *  Returns the Highest-Priority Possible Goal
     */
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

    /**
     * Get Index of the Goal Tile, according to the goal to be pursued
     */
    private int GetGoalNodeIndex(Goal goal, List<GraphNode> list)
    { 
        if (goal.GetType() == typeof(AttackEnemyGoal) || goal.GetType() == typeof(ExplodeBlockGoal))
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

    //Resets the agent's simulated position, in order to be once again equal to the real position
    private void ResetSim()
    {
        simulatedX = position.x;
        simulatedY = position.y;
    }

    /**
     * Method to Print a Plan
     * List<SymbolicAction> plan: The plan to be printed
     */
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

    //Method to deal with the agent's death
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {
        //na função AgentCall a interface vai lidar com decrementar a sua variável que indica o numero de jogadores
        updateInterface.AgentCall(this, g, prng);
    }

}

