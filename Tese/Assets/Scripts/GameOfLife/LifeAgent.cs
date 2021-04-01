using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Agent that represents one cell of a Game of Life game scenario
//Can either be "dead" or "alive"
public class LifeAgent : Agent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public LifeAgent(List<int> states, int x, int y) {

        //states[0] - number of alive neighours 
        //states[1] - alive/dead
        this.states = states;

        this.position = new Vector2Int(x,y);
        this.typeName = "Live_Agent";

        //relative locations of neighbours
        this.relative_sensors = new List<Vector2Int> {
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)};

        this.constant_sensors = new List<Vector2Int> { };
    }

    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //Agent follows a different set of rules depending on the given step_stage
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        switch (step_stage)
        {
            //If this function is called on the first stage of an update loop, 
            //the number of LifeAgent neighbours with states[1]=1 is tallied and stored in this Agent's states[0] to be used on the second stage of the update loop
            case 0:
                List<Agent> sensors = GetSensors(g);
                states[0] = 0;
                foreach (Agent sensor in sensors)
                {
                    if (sensor.states[1] == 1) states[0]++;
                }
                break;

            //If this function is called on the second stage of the update loop,
            //this Agent's states[1] is updated acording with the value of states[0] (following the rules of Conway's Game of Life)
            case 1:
                if (states[1] == 1)
                {
                    if (states[0] < 2 || states[0] > 3) states[1] = 0;
                    else states[1] = 1;
                }
                else
                {
                    if (states[0] == 3) states[1] = 1;
                    else states[1] = 0;
                }
                break;
        }

    }

}
