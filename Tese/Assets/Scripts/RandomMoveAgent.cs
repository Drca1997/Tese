using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Agent that in each update moves randomly
public class RandomMoveAgent : Agent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public RandomMoveAgent(List<int> states, int x, int y)
    {

        //states[0] - move direction: 0-left 1-right 2-up 3-down 
        //states[1] - probability of changing direction
        //states[2] - probability of changing direction delta between time steps
        this.states = states;
        this.position = new Vector2Int(x, y);
        this.typeName = "Random_Move_Agent";

        //This Agent's rules don't involve external input
        this.relative_sensors = new List<Vector2Int> {};
        this.constant_sensors = new List<Vector2Int> { };
    }

    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //Updates the Agent's internal states and moves in a direction acording to them
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        //The states are updated
        //states[0] concerns the direction in which the Agent will move next
        //states[1] concerns the probability of changing direction
        //states[2] concerns the increment to states[1] each time direction isn't changed
        if (prng.Next(0, 100) < states[1])
        {
            //The next direction will be one of 3 (the current one is excluded)
            states[0] = Utils.LoopInt(0,4, states[0]+ prng.Next(1, 4));
            //If the increment to the probability of changing directon is different than 0, the probability is restarted at 0
            if (states[2] != 0) states[1] = 0;
        }
        //If direction isn't changed, increment the probability of it changing next time
        else { states[1] += states[2]; }

        //Calculate the new position acording to the direction the Agent is moving
        Vector2Int newPosition = position;
        switch (states[0])
        {
            case 0:
                newPosition.x = Utils.LoopInt(0, g.width, newPosition.x - 1);
                break;
            case 1:
                newPosition.x = Utils.LoopInt(0, g.width, newPosition.x + 1);
                break;
            case 2:
                newPosition.y = Utils.LoopInt(0, g.height, newPosition.y + 1);
                break;
            case 3:
                newPosition.y = Utils.LoopInt(0, g.height, newPosition.y - 1);
                break;
        }

        //Move the Agent
        MoveAgent(newPosition, this, g);
    }


}
