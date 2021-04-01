using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple movement Agent controled by the player
public class PlayerMovementAgent : AgentPlayer
{
    //Reference for the MonoBehaviour used to start coroutines
    public MonoBehaviour mono;

    //Constructor
    //Receives List<int> (states), int (x), int (y), and MonoBehaviour (mono)
    public PlayerMovementAgent(List<int> states, int x, int y, MonoBehaviour mono)
    {

        this.states = states;
        this.position = new Vector2Int(x, y);
        this.mono = mono;
        this.typeName = "Player_Movement_Agent";

        //Since the player will be the one deciding what actions to take, no sensors are needed for this Agent
        this.relative_sensors = new List<Vector2Int> { };
        this.constant_sensors = new List<Vector2Int> { };
    }

    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //When called, meaning that is this Agent's turn to be updated, it will start a coroutine awaiting player input 
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        //If, when called, the Agent has finished its last update cycle (meaning that it is ready to start a new one)
        if (updated)
        {
            //This boolean will only return true once the agent has been fully updated
            updated = false;
            //Start the coroutine of the Agent logic. In this case, the Agent can only move arround
            mono.StartCoroutine(Move(g, step_stage, prng));
        }
    }

    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //Returns IEnumerator
    //Ienumerator function used for coroutine of the logic of the Agent
    //Will await Input from the player and move on the indicated direction
    private IEnumerator Move(Grid g, int step_stage, System.Random prng)
    {
        //Wait for one of the 4 arrow keys to be pressed
        yield return mono.StartCoroutine(WaitForKeyDown(new KeyCode[]{KeyCode.UpArrow,KeyCode.DownArrow,KeyCode.RightArrow,KeyCode.LeftArrow}));
        Vector2Int newPosition = position;
        //Calculate the new position acording to the input
        switch (input)
        {
            case KeyCode.UpArrow:
                newPosition.y = Utils.LoopInt(0, g.height, newPosition.y + 1);
                break;
            case KeyCode.DownArrow:
                newPosition.y = Utils.LoopInt(0, g.height, newPosition.y - 1);
                break;
            case KeyCode.LeftArrow:
                newPosition.x = Utils.LoopInt(0, g.width, newPosition.x - 1);
                break;
            case KeyCode.RightArrow:
                newPosition.x = Utils.LoopInt(0, g.width, newPosition.x + 1);
                break;
        }

        //Move the Agent
        MoveAgent(newPosition, this, g);

        //Clear the input
        input = KeyCode.None;

        //The Agent is now fully updated
        updated = true;
    }

    
}
