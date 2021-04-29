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

    public MLAgent MlAgentRef { get => mlAgentRef; set => mlAgentRef = value; }
    public int HeuristicAction { get => heuristicAction; set => heuristicAction = value; }

    public MLSyntheticPlayer(List<int> states, int x, int y, IUpdate updateInterface, MLAgent agentRef) : base(states, x, y, updateInterface)
    {
        MlAgentRef = agentRef;
        mlAgentRef.OnInputReceived += OnInput;
        MlAgentRef.X = position.x;
        MlAgentRef.Y = position.y;
       
        updateInterface.OnMLAgentWin += OnWin;
        heuristicMode = MlAgentRef.gameObject.GetComponent<BehaviorParameters>().IsInHeuristicMode();
        MlAgentRef.MlPlayer = this;
        tilesWithAgents = new int[6] { (int)Tile.PlayerEnemy, (int)Tile.AIEnemy, (int)Tile.PlayerEnemyNBomb, (int)Tile.AIEnemyNBomb, (int)Tile.FireNBombNPlayerEnemy, (int)Tile.FireNBombNAIEnemy};
        formerClosestDistToEnemy = int.MaxValue;
        currentClosestDistToEnemy = int.MaxValue;
        beforeFirstTurn = 0;
        finishedHeuristic = false;
        //updated = false;
    }
    

    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        //podes usar g.agentGrid para obter a matriz de List<Agent>
        //g.ConvertAgentGrid() devolve-te uma List<int>[,] correspondente
        //tem em conta que algumas das listas podem estar vazias ou ter multiplos elementos, visto que h� sitios na grelha sem ou com multiplos agentes
        //Os inteiros correspondem aos indices que os tipos de agente ocupam em g.agentTypes - podes modific�-los na interface de setup, na cria��o da nova Grid

        //Para mover o agente e criar uma bomba podes consultar o meu codigo em PBomberman.cs na fun��o Logic
        Debug.Log("Turn: " + beforeFirstTurn);
        if (heuristicMode)
        {
            //Debug.Log("HEURISTIC MODE ON");
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
        RewardGetCloserToEnemy(action); //Deteta se agente se moveu para mais perto de um inimigo e d� recompensa
        RewardIsNotSafe(action);
        Debug.Log("Penaliza��o de Itera��o: -0.1");
        MlAgentRef.SetReward(-0.1f); //PENALIZACAO por cada itera��o: -0.1
    }

    //Deteta se agente se moveu para mais perto de um inimigo e d� recompensa
    private void RewardGetCloserToEnemy(int action)
    {
        if (action < (int)Action.PlantBomb) //se ac��o for de movimento
        {
            currentClosestDistToEnemy = SyntheticPlayerUtils.GetDistToClosestEnemy(gridArray, new int[2] { position.x, position.y }, tilesWithAgents);
            if (currentClosestDistToEnemy < formerClosestDistToEnemy)
            {
                MlAgentRef.SetReward(0.002f); //RECOMPENSA por se ter aproximado de inimigo: 0.002
                formerClosestDistToEnemy = currentClosestDistToEnemy;
                Debug.Log("Recompensa por se ter aproximado de inimigo: 0.002");
            }
            else if(currentClosestDistToEnemy > formerClosestDistToEnemy)
            {
                MlAgentRef.SetReward(-0.002f); //PENALIZA�AO por se ter afastado de inimigo: -0.002
                formerClosestDistToEnemy = currentClosestDistToEnemy;
                Debug.Log("Penaliza��o por se ter afastado de inimigo: -0.002");
            }
        }
    }
    private void RewardIsNotSafe(int action)
    {
        if (action < (int)Action.PlantBomb) //se ac��o for de movimento
        {
            if (!SyntheticPlayerUtils.IsTileSafe(gridArray, new int[2] { position.x, position.y }))
            {
                MlAgentRef.SetReward(-0.0001f); //PENALIZA��O  por estar numa tile ao alcance de uma bomba: -0.0001
                Debug.Log("PENALIZA��O  por estar numa tile ao alcance de uma bomba: -0.0001");
            }
        }
    }

    public void RewardExplodeBlock()
    {
        mlAgentRef.SetReward(0.1f); //RECOMPENSA de destruir bloco: 0.1 
        Debug.Log("RECOMPENSA de destruir bloco: 0.1 ");
    }

    public void RewardKillEnemy()
    {
        mlAgentRef.SetReward(0.75f); //RECOMPENSA de matar inimigo: 0.75
        Debug.Log("RECOMPENSA de matar inimigo: 0.75");
    }

    private void OnWin(object sender, EventArgs e)
    {
        MlAgentRef.SetReward(1f); //RECOMPENSA por ganhar: 1
        Debug.Log("MLAgent Ganhou Jogo! Reward: +1.0");
        updateInterface.OnMLAgentWin -= OnWin;
        MlAgentRef.EndEpisode();
    }

    private void OnDeath()
    {
        MlAgentRef.SetReward(-1f); //PENALIZACAO por morrer: -1
        Debug.Log("Penaliza��o por morrer: -1");
        MlAgentRef.EndEpisode();
    }

    #endregion

    private void UpdateDistanceToClosestEnemy()
    {
        if (beforeFirstTurn == 0) //calcula distancia ao inimigo mais pr�ximo antes de come�ar o jogo
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

    //usa para algo que querias que o agente fa�a ao ser removido da grid
    //de momento meti codigo para o agente avisar a interface de update que "morreu", para se saber quando a simula��o deve ser parada
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {
        OnDeath();

        //na fun��o AgentCall a interface vai lidar com decrementar a sua vari�vel que indica o numero de jogadores
        updateInterface.AgentCall(this, g, prng);
    }
}
