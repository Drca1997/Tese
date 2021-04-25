using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MLAgent : Agent
{
    public enum Tile
    {
        Player, PlayerEnemy, AIEnemy, Walkable, Explodable, Unsurpassable, Bomb, Fire,
        PlayerNBomb, PlayerEnemyNBomb, AIEnemyNBomb,
        FireNExplodable, FireNPlayer, FireNPlayerEnemy, FireNAIEnemy, FireNBomb,
        FireNBombNPlayer, FireNBombNPlayerEnemy, FireNBombNAIEnemy
    }

    //private int [,] gameWorld;

    //public int [,] GameWorld { get => gameWorld; set => gameWorld = value; }

    private int x;
    private int y;
    private int rawAction;
    private KeyCode input = KeyCode.None;
    private int[,] grid;
    private MLSyntheticPlayer mlPlayer;

    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public int RawAction { get => rawAction; }
    public int[,] Grid { get => grid; set => grid = value; }
    public MLSyntheticPlayer MlPlayer { get => mlPlayer; set => mlPlayer = value; }

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
        base.OnActionReceived(vectorAction);
        rawAction = vectorAction.DiscreteActions[0];

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
        base.Heuristic(actionsOut);
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            discreteActions[0] = 0;
            Debug.Log("UP PRESSED");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)){
            discreteActions[0] = 1;
            Debug.Log("DOWN PRESSED");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            discreteActions[0] = 2;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            discreteActions[0] = 3;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            discreteActions[0] = 4;
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            discreteActions[0] = 5;
        }

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
