using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Agent of a Bomberman game scenario that represents a unbreakable wall
public class AStrongWall : Agent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public AStrongWall(List<int> states, int x, int y)
    {
        this.states = states;
        this.position = new Vector2Int(x, y);
        this.typeName = "Agent_Strong_Wall";

        //the update rules of this Agent are only concerned with the other Agents on its position
        this.relative_sensors = new List<Vector2Int> { };
        this.constant_sensors = new List<Vector2Int> { };
    }

    //Receives Grid(g), int (step_stage), and System.Random(prng)
    //Dosen't do anything, its an unbreakable wall
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        
    }

    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //Executed on the elimination of the Agent form the agentGrid
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {

    }
}
