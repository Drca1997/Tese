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
    }

    /**
     * Função que recolhe observações do ambiente
     */
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        foreach (int[] tile in SyntheticPlayerUtils.GridIterator(grid)) //Add Grid to Observation Vector
        {
            sensor.AddOneHotObservation(grid[tile[0], tile[1]], (int)Tile.FireNBombNAIEnemy); //Categorical Types are recommended to be encoded as One-Hot Observation
        }

        sensor.AddObservation(x / grid.GetLength(0)); // Add X of Agent to Observation Vector. Recommended to be normalized between 0 and 1.
        sensor.AddObservation(y / grid.GetLength(1)); // Add Y of Agent to Observation Vector. Recommended to be normalized between 0 and 1.
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
        base.WriteDiscreteActionMask(actionMask);
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
