using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Agent of a Bomberman game scenario that represents a fire
//expiers after a given number of time steps, eleminating itself from the grid
//destroys some other agent types if in the same position
public class AFire : Agent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public AFire(List<int> states, int x, int y)
    {
        //states[0] - number of updates until it expires
        this.states = states;
        this.position = new Vector2Int(x, y);
        this.typeName = "Agent_Fire";

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
        List<string> flamableTypes = new List<string> {"Agent_Weak_Wall"};
        List<Agent> sensors = GetSensors(g);
        foreach (Agent a in sensors)
        {
            foreach (string type in flamableTypes)
            {
                if (string.Compare(a.typeName, type) == 0)
                {
                    EliminateAgent(a, g);
                    return;
                }
            }
        }

        //Countdown until expiration
        if (states[0] > 0) states[0]--;
        else
        {
            //eleminate self
            RemoveAgentOffGrid(this, g);
        }
    }
}
