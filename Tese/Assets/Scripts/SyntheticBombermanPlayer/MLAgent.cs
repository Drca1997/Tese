using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class MLAgent : Agent
{
    public enum Tile
    {
        Player, PlayerEnemy, AIEnemy, Walkable, Explodable, Unsurpassable, Bomb, Fire,
        PlayerNBomb, PlayerEnemyNBomb, AIEnemyNBomb,
        FireNExplodable, FireNPlayer, FireNPlayerEnemy, FireNAIEnemy, FireNBomb,
        FireNBombNPlayer, FireNBombNPlayerEnemy, FireNBombNAIEnemy
    }
    public enum Action
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        PlantBomb,
        DoNothing
    }



    private int x;
    private int y;
    private int rawAction;
    private KeyCode input = KeyCode.None;
    private int[,] grid;
    private MLSyntheticPlayer mlPlayer;
    private bool start = false;
    private bool restart = false;
    public bagingus gameHandler;
    public Recompensas Recompensas;
    private const int tileVectorObservationSize = 5;
    private const int bombermanObsIndex = 0;
    private const int explodableObsIndex = 1;
    private const int unsurpassableObsIndex = 2;
    private const int bombObsIndex = 3;
    private const int fireObsIndex = 4;

    public event EventHandler OnInputReceived;
    
    
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public int RawAction { get => rawAction; }
    public int[,] Grid { get => grid; set => grid = value; }
    public MLSyntheticPlayer MlPlayer { get => mlPlayer; set => mlPlayer = value; }
    public bool Start { get => start;}


    /**
    * Função chamada quando um novo episódio de treino começa
    */
    public override void OnEpisodeBegin() 
    {
        base.OnEpisodeBegin();

        
        //gameHandler = gameObject.GetComponent<bagingus>();
        gameHandler.OnEpisodeEnd += OnEpisodeEnd;
        if (gameHandler.EpisodeNumber > 0)
        {
            StartCoroutine(WaitForEpisodeRestart());
        }
    }
    
    public IEnumerator WaitForEpisodeRestart()
    {
        while (!restart)
        {

            yield return null;
        }

        restart = false;
        //Debug.Log("New Episode with MLPlayer " + mlPlayer.id);
        //StartCoroutine(WaitForNewRef());
    }

    private void OnEpisodeEnd(object sender, EventArgs e)
    {
        gameHandler.OnEpisodeEnd -= OnEpisodeEnd;
        gameHandler.OnRestart += OnRestart;
        //Debug.Log("End of Episode");
        EndEpisode();

    }

    private void OnRestart(object sender, EventArgs e)
    {
        gameHandler.OnRestart -= OnRestart;
        //Debug.Log("Restart");
        restart = true;
    }

    /**Função que recolhe observações do ambiente
     * Vetor de Observaçao: [Bomberman, Explodable, Unsurpassable, Bomb, Fire]
     * Cada tile é caracterizada por um vetor de observação. Elementos com 1 significam que esse agente está na tile, 0 representa ausencia desse agente
     * Ex: [1, 0, 0, 0, 1] -> significa que um Bomberman e agente "Fire" estão na mesma tile, o que irá causar a morte do Bomberman em questão
     */
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        foreach (int[] tile in SyntheticPlayerUtils.GridIterator(grid)) //Add Grid to Observation Vector
        {

            int [] tileState = CollectTileObservation(tile);
            for (int i = 0; i < tileState.Length; i++)
            {
                sensor.AddObservation(tileState[i]);
            }
        }

        sensor.AddObservation(x / grid.GetLength(0)); // Add X of Agent to Observation Vector. Recommended to be normalized between 0 and 1.
        sensor.AddObservation(y / grid.GetLength(1)); // Add Y of Agent to Observation Vector. Recommended to be normalized between 0 and 1.
    }

    private int[] CollectTileObservation(int [] tile)
    {
        int[] tileState = new int[tileVectorObservationSize] {0, 0, 0, 0, 0};
        switch(grid[tile[0], tile[1]])
        {

            case (int)Tile.Player:
            case (int)Tile.PlayerEnemy:
            case (int)Tile.AIEnemy:
                tileState[bombermanObsIndex] = 1;
                break;
            case (int)Tile.Explodable:
                tileState[explodableObsIndex] = 1;
                break;
            case (int)Tile.Unsurpassable:
                tileState[unsurpassableObsIndex] = 1;
                break;
            case (int)Tile.Bomb:
                tileState[bombObsIndex] = 1;
                break;
            case (int)Tile.Fire:
                tileState[fireObsIndex] = 1;
                break;
            case (int)Tile.PlayerNBomb:
            case (int)Tile.PlayerEnemyNBomb:
            case (int)Tile.AIEnemyNBomb:
                tileState[bombermanObsIndex] = 1;
                tileState[bombObsIndex] = 1;
                break;
            case (int)Tile.FireNPlayer:
            case (int)Tile.FireNPlayerEnemy:
            case (int)Tile.FireNAIEnemy:
                tileState[bombermanObsIndex] = 1;
                tileState[fireObsIndex] = 1;
                break;
            case (int)Tile.FireNExplodable:
                tileState[explodableObsIndex] = 1;
                tileState[fireObsIndex] = 1;
                break;
            case (int)Tile.FireNBomb:
                tileState[bombObsIndex] = 1;
                tileState[fireObsIndex] = 1;
                break;
            case (int)Tile.FireNBombNPlayer:
            case (int)Tile.FireNBombNPlayerEnemy:
            case (int)Tile.FireNBombNAIEnemy:
                tileState[bombermanObsIndex] = 1;
                tileState[fireObsIndex] = 1;
                tileState[bombObsIndex] = 1;
                break;


        }
        //DebugObsVector(tileState, tile);
        return tileState;
    }

    private void DebugObsVector(int [] vector, int[] tile)
    {
        string res = null;
        for (int i=0; i < vector.Length; i++)
        {
            res += vector[i].ToString() + " ";
        }
        Debug.Log(tile[0] + ", " + tile[1] + ": " +res);
    }

    /**
     * Função evocada depois do agente tomar uma decisão
     * */
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        //Debug.Log("Recebida Açao: " + vectorAction.DiscreteActions[0]);
        rawAction = vectorAction.DiscreteActions[0];
        OnInputReceived?.Invoke(this, EventArgs.Empty);
    }

    /**
     * Impede agente de tomar decisões que não são válidas 
     */
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask){ 
        actionMask.WriteMask(0, mlPlayer.GetImpossibleActions());
    }

    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = mlPlayer.HeuristicAction;
    }


    

    //Bernardo's function of GameAgentPlayer
    public IEnumerator WaitForKeyDown(KeyCode[] codes)
    {
        bool pressed = false;
        while (!pressed)
        {
            foreach (KeyCode k in codes)
            {
                if (Input.GetKeyDown(k))
                {
                    pressed = true;
                    input = k;
                    break;
                }
            }
            yield return null;
        }
    }




}
