using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Agent of a Bomberman game scenario that represents a bomb
//explodes after a given number of time steps, eleminating itself and creating AFire Agents
public class ABomb : Agent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public ABomb(List<int> states, int x, int y)
    {
        //states[0] - number of updates until explosion
        this.states = states;
        this.position = new Vector2Int(x, y);
        this.typeName = "Agent_Bomb";

        //This Agent's update rules ar not influenced by exterior inputs
        this.relative_sensors = new List<Vector2Int> { };
        this.constant_sensors = new List<Vector2Int> { };
    }

    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //In each call the agent reduces states[0] until its value is 0, 
    //at which point it elemintaes itself and creates a number of AFire Agents in a patttern
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        //countdown until explosion
        if (states[0] > 0) states[0]--;
        else
        {
            //cross pattern
            List<Vector2Int> firePlaces = new List<Vector2Int> {
                new Vector2Int(0, 0),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0),
                new Vector2Int(0, 1) 
            };

            //eleminate self
            RemoveAgentOffGrid(this, g);

            //create new AFire Agents in the positions contained in firePlaces
            foreach(Vector2Int pos in firePlaces)
            {
                Vector2Int realPos = GetRealPos(position, pos, g);
                PutAgentOnGrid(realPos, new AFire(new List<int> {0}, realPos.x, realPos.y), g);
            }
        }
    }
}
