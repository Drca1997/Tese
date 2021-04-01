using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Agent of a Bomberman game scenario that represents a breakable wall
public class AWeakWall : Agent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public AWeakWall(List<int> states, int x, int y)
    {
        this.states = states;
        this.position = new Vector2Int(x, y);
        this.typeName = "Agent_Weak_Wall";

        //the update rules of this Agent are only concerned with the other Agents on its position
        this.relative_sensors = new List<Vector2Int> { };
        this.constant_sensors = new List<Vector2Int> { };
    }

    //Receives Grid(g), int (step_stage), and System.Random(prng)
    //
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        
    }
}
