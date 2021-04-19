using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Agent that represents a "dead" cell of a Game of Life game scenario
//Used in conjunction with LifeAgentAlive
public class LifeAgentDead : GameAgent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public LifeAgentDead(List<int> states, int x, int y)
    {

        //states[0] - number of alive neighours 
        this.states = states;
        this.position = new Vector2Int(x, y);
        this.typeName = "Live_Agent_Dead";

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
            //the number of LifeAgentAlive neighbours with is tallied and stored in this Agent's states[0] to be used on the second stage of the update loop
            case 0:
                List<GameAgent> sensors = GetSensors(g);
                states[0] = 0;
                foreach (GameAgent sensor in sensors)
                {
                    if (string.Compare(sensor.typeName, "Live_Agent_Alive")==0) states[0]++;
                }
                break;

            //If this function is called on the second stage of the update loop,
            //this Agent will eliminate itself and create an LifeAgentAlive in its place if the rules of Conway's Game of Life so call it
            case 1:
                //eliminates itself and creates an LifeAgentAlive in its place
                if (states[0] == 3)
                {
                    EliminateAgent(this, g, step_stage, prng);
                    PutAgentOnGrid(position, new LifeAgentAlive(new List<int> {0}, position.x, position.y), g);
                };
                break;
        }

    }


    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //Executed on the elimination of the Agent form the agentGrid
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {

    }
}