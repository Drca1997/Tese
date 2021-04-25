using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;

public class MLSyntheticPlayer: SyntheticBombermanPlayer
{
    private MLAgent mlAgentRef;
    private bool heuristicMode;
    private int[] tilesWithAgents;
    private int formerClosestDistToEnemy;
    private int currentClosestDistToEnemy;
    private int beforeFirstTurn;
    //private int id;
    public MLSyntheticPlayer(List<int> states, int x, int y, IUpdate updateInterface, MLAgent agentRef) : base(states, x, y, updateInterface)
    {
        mlAgentRef = agentRef;
        mlAgentRef.X = position.x;
        mlAgentRef.Y = position.y;
        
       
        updateInterface.OnMLAgentWin += OnWin;
        heuristicMode = mlAgentRef.gameObject.GetComponent<BehaviorParameters>().IsInHeuristicMode();
        //id = mlAgentRef.gameObject.GetComponent<BehaviorParameters>().TeamId;
        mlAgentRef.MlPlayer = this;
        tilesWithAgents = new int[6] { (int)Tile.PlayerEnemy, (int)Tile.AIEnemy, (int)Tile.PlayerEnemyNBomb, (int)Tile.AIEnemyNBomb, (int)Tile.FireNBombNPlayerEnemy, (int)Tile.FireNBombNAIEnemy};
        formerClosestDistToEnemy = int.MaxValue;
        currentClosestDistToEnemy = int.MaxValue;
        beforeFirstTurn = 0;
    }
    

    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        //podes usar g.agentGrid para obter a matriz de List<Agent>
        //g.ConvertAgentGrid() devolve-te uma List<int>[,] correspondente
        //tem em conta que algumas das listas podem estar vazias ou ter multiplos elementos, visto que há sitios na grelha sem ou com multiplos agentes
        //Os inteiros correspondem aos indices que os tipos de agente ocupam em g.agentTypes - podes modificá-los na interface de setup, na criação da nova Grid

        //Para mover o agente e criar uma bomba podes consultar o meu codigo em PBomberman.cs na função Logic
        if (heuristicMode)
        {
            Debug.Log("HEURISTIC");
        }
        else
        {
            if (beforeFirstTurn == 0) //calcula distancia ao inimigo mais próximo antes de começar o jogo
            {
                formerClosestDistToEnemy = SyntheticPlayerUtils.GetDistToClosestEnemy(gridArray, new int[2] { position.x, position.y }, tilesWithAgents);
                currentClosestDistToEnemy = formerClosestDistToEnemy;
                beforeFirstTurn++;
            }
            Debug.Log("UPDATING MLAGENT in " + position.x + ", " + position.y);
            HasBomb();
            gridArray = ConvertGrid(g);

            mlAgentRef.X = position.x;
            mlAgentRef.Y = position.y;
            int actionTaken = TakeAction();
            ProcessAction(g, actionTaken);

            CalculateReward(actionTaken);
        }
        
    }
    public override int TakeAction()
    {
        mlAgentRef.RequestDecision();
        Debug.Log(SyntheticPlayerUtils.ActionToString(mlAgentRef.RawAction));
        return mlAgentRef.RawAction;
    }

    private void CalculateReward(int action)
    {
        RewardGetCloserToEnemy(action); //Deteta se agente se moveu para mais perto de um inimigo e dá recompensa
        RewardIsNotSafe(action);
        mlAgentRef.SetReward(-0.1f); //PENALIZACAO por cada iteração: -0.1
    }

    //Deteta se agente se moveu para mais perto de um inimigo e dá recompensa
    private void RewardGetCloserToEnemy(int action)
    {
        if (action < (int)Action.PlantBomb) //se acção for de movimento
        {
            currentClosestDistToEnemy = SyntheticPlayerUtils.GetDistToClosestEnemy(gridArray, new int[2] { position.x, position.y }, tilesWithAgents);
            if (currentClosestDistToEnemy < formerClosestDistToEnemy)
            {
                mlAgentRef.SetReward(0.002f); //RECOMPENSA por se ter aproximado de inimigo: 0.002
                formerClosestDistToEnemy = currentClosestDistToEnemy;
            }
        }
    }
    private void RewardIsNotSafe(int action)
    {
        if (action < (int)Action.PlantBomb) //se acção for de movimento
        {
            if (!SyntheticPlayerUtils.IsTileSafe(gridArray, new int[2] { position.x, position.y }))
            {
                mlAgentRef.SetReward(-0.0001f); //PENALIZAÇÃO  por estar numa tile ao alcance de uma bomba: -0.0001
            }
        }
    }

    private void OnWin(object sender, EventArgs e)
    {
        mlAgentRef.SetReward(1f); //RECOMPENSA por ganhar: 1
        Debug.Log("MLAgent Ganhou Jogo! Reward: +1.0");
        updateInterface.OnMLAgentWin -= OnWin;
        mlAgentRef.EndEpisode();
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
        mlAgentRef.SetReward(-0.5f); //PENALIZACAO por morrer: -0.5
        mlAgentRef.EndEpisode();

        //na função AgentCall a interface vai lidar com decrementar a sua variável que indica o numero de jogadores
        updateInterface.AgentCall(this, g, prng);
    }
}
