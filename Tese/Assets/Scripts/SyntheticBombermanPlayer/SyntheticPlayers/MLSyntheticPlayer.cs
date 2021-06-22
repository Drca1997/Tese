using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using System.Collections;

public class MLSyntheticPlayer: SyntheticBombermanPlayer
{
    private MLAgent mlAgentRef;
    private bool heuristicMode;
    private int[] tilesWithAgents;
    private int formerClosestDistToEnemy;
    private int currentClosestDistToEnemy;
    private int beforeFirstTurn;
    private int heuristicAction;
    private bool finishedHeuristic;
    public static int nextId = 0;
    public int id;
    private int teamID;

    public MLAgent MlAgentRef { get => mlAgentRef; set => mlAgentRef = value; }
    public int HeuristicAction { get => heuristicAction; set => heuristicAction = value; }
    public int TeamID { get => teamID; }

    private Recompensas recompensas;
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
    

    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        //podes usar g.agentGrid para obter a matriz de List<Agent>
        //g.ConvertAgentGrid() devolve-te uma List<int>[,] correspondente
        //tem em conta que algumas das listas podem estar vazias ou ter multiplos elementos, visto que há sitios na grelha sem ou com multiplos agentes
        //Os inteiros correspondem aos indices que os tipos de agente ocupam em g.agentTypes - podes modificá-los na interface de setup, na criação da nova Grid

        //Para mover o agente e criar uma bomba podes consultar o meu codigo em PBomberman.cs na função Logic
        Debug.Log("Turn: " + beforeFirstTurn);
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

    private void UpdateAgentInternalState(Grid g)
    {
        HasBomb();
        gridArray = ConvertGrid(g);
        mlAgentRef.Grid = gridArray;
        UpdateDistanceToClosestEnemy();

        MlAgentRef.X = position.x;
        MlAgentRef.Y = position.y;
    }

    private void DecisionMakingProcess(Grid g)
    {
        int actionTaken = TakeAction();
        ProcessAction(g, actionTaken);
        CalculateReward(actionTaken);
    }

    public override int TakeAction()
    {
        if (!heuristicMode)
            MlAgentRef.RequestDecision();

        Debug.Log(SyntheticPlayerUtils.ActionToString(MlAgentRef.RawAction));
        return MlAgentRef.RawAction;
    }

    #region HeuristicMode
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

    private void OnInput(object sender, EventArgs e)
    {
        finishedHeuristic = true;
    }

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

    private void RewardIsNotSafe(int action)
    {
        if (!SyntheticPlayerUtils.IsTileSafe(gridArray, new int[2] { position.x, position.y }))
        {
            MlAgentRef.SetReward(recompensas.NotSafePenaltyValue); //PENALIZAÇÃO  por estar numa tile ao alcance de uma bomba: -0.0001
            Debug.Log("PENALIZAÇÃO  por estar numa tile ao alcance de uma bomba: " + recompensas.NotSafePenaltyValue);
        }   
    }

    public void RewardExplodeBlock()
    {
        if (recompensas.ExplodeBlockReward)
        {
            mlAgentRef.SetReward(recompensas.ExplodeBlockRewardValue); //RECOMPENSA de destruir bloco: 0.1 
            Debug.Log("RECOMPENSA de destruir bloco:  " + recompensas.ExplodeBlockRewardValue);
        }
       
    }

    public void RewardKillEnemy()
    {
        if (recompensas.KillEnemyReward)
        {
            mlAgentRef.SetReward(recompensas.KillEnemyRewardValue); //RECOMPENSA de matar inimigo: 0.75
            Debug.Log("RECOMPENSA de matar inimigo: " + recompensas.KillEnemyRewardValue);
        }
        
    }

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

    private void OnDeath()
    {
        MlAgentRef.SetReward(recompensas.OnDeathPenaltyValue); //PENALIZACAO por morrer: -1
        Debug.Log("Penalização por morrer: " + recompensas.OnDeathPenaltyValue);
        //MlAgentRef.EndEpisode();
    }

    #endregion

    private void UpdateDistanceToClosestEnemy()
    {
        if (beforeFirstTurn == 0) //calcula distancia ao inimigo mais próximo antes de começar o jogo
        {
            formerClosestDistToEnemy = SyntheticPlayerUtils.GetDistToClosestEnemy(gridArray, new int[2] { position.x, position.y }, tilesWithAgents);
            currentClosestDistToEnemy = formerClosestDistToEnemy;

        }
        beforeFirstTurn++;
    }

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

    //usa para algo que querias que o agente faça ao ser removido da grid
    //de momento meti codigo para o agente avisar a interface de update que "morreu", para se saber quando a simulação deve ser parada
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {
        if (recompensas.OnDeathPenalty)
            OnDeath();

        //na função AgentCall a interface vai lidar com decrementar a sua variável que indica o numero de jogadores
        updateInterface.AgentCall(this, g, prng);
    }
}
