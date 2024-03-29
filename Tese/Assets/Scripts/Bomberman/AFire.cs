using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Agent of a Bomberman game scenario that represents a fire
//expiers after a given number of time steps, eleminating itself from the grid
//destroys some other agent types if in the same position
public class AFire : GameAgent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public AFire(List<int> states, int x, int y, GameAgent creator)
    {
        //states[0] - number of updates until it expires
        this.states = states;
        this.position = new Vector2Int(x, y);
        this.typeName = "Agent_Fire";
        this.creator = creator;

        //the update rules of this Agent are only concerned with the other Agents on its position
        this.relative_sensors = new List<Vector2Int> { new Vector2Int(0, 0) };
        this.constant_sensors = new List<Vector2Int> { };
    }

    //Receives Grid(g), int (step_stage), and System.Random(prng)
    //In each call the agent reduces states[0] until its value is 0, 
    //at which point it elemintaes itself
    //In the mean time, if some specific types of agents are in its position of the agentGrid, they will be eliminated 
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        //Will remove Agents of these types if they are in its postion of the agentGrid
        List<string> flamableTypes = new List<string> {"Agent_Weak_Wall", "Malaquias_Bomberman", "Player_Bomberman", "Agent_Bomberman" };
        List<GameAgent> sensors = GetSensors(g);
        foreach (GameAgent a in sensors)
        {
            foreach (string type in flamableTypes)
            {
                if (string.Compare(a.typeName, type) == 0)
                {
                    //DIOGO - podes aqui verificar que tipo de agente foi destruido 
                    //Este agente AFire tem uma componente creator - a bomba (ABomb) que o criou
                    //a ABomb tambem tem uma componente creator que referencia o jogador que a colocou - ou seja, creator.creator
                    //podes a partir disso adicionar-lhe score se for um sintético 
                    EliminateAgent(a, g, step_stage, prng);
                }
            }
        }

        //Countdown until expiration
        if (states[0] > 0) states[0]--;
        else
        {
            //eleminate self
            EliminateAgent(this, g, step_stage, prng);
        }
    }



    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //Executed on the elimination of the Agent form the agentGrid
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {

    }
}
