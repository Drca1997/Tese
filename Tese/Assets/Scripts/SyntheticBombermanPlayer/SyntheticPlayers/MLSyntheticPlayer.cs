using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using System.Collections;

public class MLSyntheticPlayer: SyntheticBombermanPlayer
{
    private MLAgent mlAgentRef; //reference to the MLAgent file in charge of the machine learning training process
    private bool heuristicMode; //boolean than indicates if the agent is running in heuristic mode (human controlling it to provide demonstrations) or not
    private int[] tilesWithAgents; //vector with the tile configurations that indicates an agent in them
    private int formerClosestDistToEnemy; //Distance to the closest enemy before executing the most recent action. Used to calculate the reward/penalty of being closer or farther away from an enemy 
    private int currentClosestDistToEnemy; //Distance to the closest enemy after executing the most recent action. Used to calculate the reward/penalty of being closer or farther away from an enemy 
    private int beforeFirstTurn; 
    private int heuristicAction;
    private bool finishedHeuristic; //boolean that indicates if the heuristic loop of the ML Agent script ended in this turn. Used in a Coroutine to synchronize both scripts
    public static int nextId = 0;  //ID to be given to the next instance of this agent.
    public int id; //id of the agent. This only useful to a matter of debug, to check if there is a new instance of the agent being created at every game session
    private int teamID; //The TeamID parameter of the ML Agent script

    public MLAgent MlAgentRef { get => mlAgentRef; set => mlAgentRef = value; }
    public int HeuristicAction { get => heuristicAction; set => heuristicAction = value; }
    public int TeamID { get => teamID; }

    private Recompensas recompensas; //Reference to the script responsible for managing the rewards setup

    /**
     * Constructor of an Instance of this Agent
     * List<int> states: an intrinsic attribute of a platform's game agent. This particular agent does not use this
     * int x: the x coordinate of the agent’s position in the grid 
     * int y: the y coordinate of the agent’s position in the grid
     * IUpdate updateInterface: reference to the platform’s update interface, responsible for the game loop update.
     * MLAgent agentRef: reference to the MLAgent file in charge of the machine learning training process
    */
    public MLSyntheticPlayer(List<int> states, int x, int y, IUpdate updateInterface, MLAgent agentRef) : base(states, x, y, updateInterface)
    {
        MlAgentRef = agentRef;
        id = nextId;
        //Debug.Log("Criado ML " + id);
        nextId++;
        mlAgentRef.OnInputReceived += OnInput;
        MlAgentRef.X = position.x;
        MlAgentRef.Y = position.y;
        Debug.Log("TEAM ID: " + MlAgentRef.gameObject.GetComponent<BehaviorParameters>().TeamId);
        teamID = MlAgentRef.gameObject.GetComponent<BehaviorParameters>().TeamId;
        updateInterface.OnMLAgentWin += OnWin;
        heuristicMode = MlAgentRef.gameObject.GetComponent<BehaviorParameters>().IsInHeuristicMode();
        MlAgentRef.MlPlayer = this;
        //Debug.Log("New Ref:" + mlAgentRef.MlPlayer.id);
        
        tilesWithAgents = new int[6] { (int)Tile.PlayerEnemy, (int)Tile.AIEnemy, (int)Tile.PlayerEnemyNBomb, (int)Tile.AIEnemyNBomb, (int)Tile.FireNBombNPlayerEnemy, (int)Tile.FireNBombNAIEnemy};
        formerClosestDistToEnemy = int.MaxValue;
        currentClosestDistToEnemy = int.MaxValue;
        beforeFirstTurn = 0;
        finishedHeuristic = false;
        recompensas = mlAgentRef.Recompensas;
        //updated = false;
    }


    //Method responsible for updating the agent in its turn
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        if (heuristicMode)
        {
            Debug.Log("HEURISTIC MODE ON");
            if (updated)
            {
                //This boolean will only return true once the agent has been fully updated
                updated = false;
                mlAgentRef.StartCoroutine(InputLogic(g, step_stage, prng));
            }
        }
        else
        {
            UpdateAgentInternalState(g);
            DecisionMakingProcess(g);
        }
    }

    //Update Internal attributes of the agent
    private void UpdateAgentInternalState(Grid g)
    {
        HasBomb();
        gridArray = ConvertGrid(g);
        mlAgentRef.Grid = gridArray;
        UpdateDistanceToClosestEnemy();

        MlAgentRef.X = position.x;
        MlAgentRef.Y = position.y;
    }

    //Decides Action to Take + Executes Action in the Environment + Calculate the Obtained Reward with the Action
    private void DecisionMakingProcess(Grid g)
    {
        int actionTaken = TakeAction();
        ProcessAction(g, actionTaken);
        CalculateReward(actionTaken);
    }

    //Returns what action to execute
    public override int TakeAction()
    {
        if (!heuristicMode)
            MlAgentRef.RequestDecision();

        Debug.Log(SyntheticPlayerUtils.ActionToString(MlAgentRef.RawAction));
        ReactionTime();
        return MlAgentRef.RawAction;
    }

    #region HeuristicMode
    //Deals with the game Input Logic in case of heuristic mode is on
    private IEnumerator InputLogic(Grid g, int step_stage, System.Random prng)
    {
        
        //Wait for one of the 4 arrow keys, the spacebar or N to be pressed
        yield return mlAgentRef.StartCoroutine(WaitForKeyDown(new KeyCode[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.Space, KeyCode.N }));
        
        switch (input)
        {
            case KeyCode.UpArrow:
                HeuristicAction = (int)Action.MoveUp;
                break;
            case KeyCode.DownArrow:
                HeuristicAction = (int)Action.MoveDown;
                break;
            case KeyCode.LeftArrow:
                HeuristicAction = (int)Action.MoveLeft;
                break;
            case KeyCode.RightArrow:
                HeuristicAction = (int)Action.MoveRight;
                break;
            case KeyCode.Space:
                HeuristicAction = (int)Action.PlantBomb;
                break;
            case KeyCode.N:
                HeuristicAction = (int)Action.DoNothing;
                break;
        }

        //Clear the input
        input = KeyCode.None;

        

        UpdateAgentInternalState(g);
        mlAgentRef.RequestDecision();
        yield return mlAgentRef.StartCoroutine(WaitForHeuristic(g));

        //The Agent is now fully updated
        updated = true;

    }


    //Activated when the OnInput Event is fired
    private void OnInput(object sender, EventArgs e)
    {
        finishedHeuristic = true;
    }

    /**
     * Synchronizes both scripts
    */
    private IEnumerator WaitForHeuristic(Grid g)
    {
        while (!finishedHeuristic)
        {

            yield return null;
        }


        DecisionMakingProcess(g);

        finishedHeuristic = false;
    }
    #endregion

    #region Recompensas
    //Claculates all the rewards
    private void CalculateReward(int action)
    {
        if (recompensas.GetCloserToEnemyReward)
            RewardGetCloserToEnemy(action); //Deteta se agente se moveu para mais perto de um inimigo e dá recompensa
        if (recompensas.NotSafePenalty)
            RewardIsNotSafe(action);
        if (recompensas.IterationPenalty)
        {
            //Debug.Log("Penalização de Iteração: " + recompensas.IterationPenaltyValue);
            MlAgentRef.SetReward(recompensas.IterationPenaltyValue); //PENALIZACAO por cada iteração: -0.01
        }
       
    }

    //Deteta se agente se moveu para mais perto de um inimigo e dá recompensa
    private void RewardGetCloserToEnemy(int action)
    {
        if (action < (int)Action.PlantBomb) //se acção for de movimento
        {
            currentClosestDistToEnemy = SyntheticPlayerUtils.GetDistToClosestEnemy(gridArray, new int[2] { position.x, position.y }, tilesWithAgents);
            //Debug.Log("Current Dist: " + currentClosestDistToEnemy);
            //Debug.Log("Former Dist: " + formerClosestDistToEnemy);
            if (currentClosestDistToEnemy < formerClosestDistToEnemy)
            {
                MlAgentRef.SetReward(recompensas.GetCloserToEnemyRewardValue); //RECOMPENSA por se ter aproximado de inimigo: 0.002
                formerClosestDistToEnemy = currentClosestDistToEnemy;
                Debug.Log("Recompensa por se ter aproximado de inimigo: " + recompensas.GetCloserToEnemyRewardValue);
            }
            else if(currentClosestDistToEnemy > formerClosestDistToEnemy)
            {
                MlAgentRef.SetReward(recompensas.GetAwayFromEnemyPenaltyValue); //PENALIZAÇAO por se ter afastado de inimigo: -0.002
                formerClosestDistToEnemy = currentClosestDistToEnemy;
                Debug.Log("Penalização por se ter afastado de inimigo: " + recompensas.GetAwayFromEnemyPenaltyValue);
            }
        }
    }

    //Reward for agent being safe or not
    private void RewardIsNotSafe(int action)
    {
        if (!SyntheticPlayerUtils.IsTileSafe(gridArray, new int[2] { position.x, position.y }))
        {
            MlAgentRef.SetReward(recompensas.NotSafePenaltyValue); //PENALIZAÇÃO  por estar numa tile ao alcance de uma bomba: -0.0001
            Debug.Log("PENALIZAÇÃO  por estar numa tile ao alcance de uma bomba: " + recompensas.NotSafePenaltyValue);
        }   
    }

    //Reward for exploding a block
    public void RewardExplodeBlock()
    {
        if (recompensas.ExplodeBlockReward)
        {
            mlAgentRef.SetReward(recompensas.ExplodeBlockRewardValue); //RECOMPENSA de destruir bloco: 0.1 
            Debug.Log("RECOMPENSA de destruir bloco:  " + recompensas.ExplodeBlockRewardValue);
        }
       
    }

    //Reward for killing an enemy
    public void RewardKillEnemy()
    {
        if (recompensas.KillEnemyReward)
        {
            mlAgentRef.SetReward(recompensas.KillEnemyRewardValue); //RECOMPENSA de matar inimigo: 0.75
            Debug.Log("RECOMPENSA de matar inimigo: " + recompensas.KillEnemyRewardValue);
        }
        
    }

    //Activated when the agent wins the game. Agent gets a positive reward
    private void OnWin(object sender, EventArgs e)
    {
        if (recompensas.OnWinReward)
        {
            MlAgentRef.SetReward(recompensas.OnWinRewardValue); //RECOMPENSA por ganhar: 1
            Debug.Log("MLAgent Ganhou Jogo! Reward: " + recompensas.OnWinRewardValue);
        }
       
        updateInterface.OnMLAgentWin -= OnWin;
        //MlAgentRef.EndEpisode();
    }

    //Activated when the agent dies. Agent gets a negative reward
    private void OnDeath()
    {
        MlAgentRef.SetReward(recompensas.OnDeathPenaltyValue); //PENALIZACAO por morrer: -1
        Debug.Log("Penalização por morrer: " + recompensas.OnDeathPenaltyValue);
        //MlAgentRef.EndEpisode();
    }

    #endregion

    //Update the distance to the closest enemy after executing the most recent action
    private void UpdateDistanceToClosestEnemy()
    {
        if (beforeFirstTurn == 0) //calcula distancia ao inimigo mais próximo antes de começar o jogo
        {
            formerClosestDistToEnemy = SyntheticPlayerUtils.GetDistToClosestEnemy(gridArray, new int[2] { position.x, position.y }, tilesWithAgents);
            currentClosestDistToEnemy = formerClosestDistToEnemy;

        }
        beforeFirstTurn++;
    }

    //Get impossible actions to execute in the current turn
    public IEnumerable<int> GetImpossibleActions()
    {
        List<int> possibleActions = new List<int>();
        for (int i=0; i <= (int)Action.DoNothing; i++)
        {
            if (!SyntheticPlayerUtils.IsValidAction(gridArray, this, i))
                possibleActions.Add(i);
        }
        return possibleActions;
    }

    //Platform's method to deal with this game agent's death
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {
        if (recompensas.OnDeathPenalty)
            OnDeath();

        //na função AgentCall a interface vai lidar com decrementar a sua variável que indica o numero de jogadores
        updateInterface.AgentCall(this, g, prng);
    }
}
