using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Agent of a Bomberman game scenario that represents a bomb
//explodes after a given number of time steps, eleminating itself and creating AFire Agents
public class ABomb : GameAgent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public ABomb(List<int> states, int x, int y, GameAgent creator)
    {
        //states[0] - number of updates until explosion
        //states[1] - size of explosion
        if (states.Count == 2)
        {
            this.states = states;
        }
        else this.states = new List<int> { 3, 2 };
        this.position = new Vector2Int(x, y);
        this.typeName = "Agent_Bomb";
        this.creator = creator;

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
            //eleminate self
            EliminateAgent(this, g, step_stage, prng);

        }
    }

    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //Executed on the elimination of the Agent form the agentGrid
    //Bomb creates a number of AFire Agents in a patttern on elimination 
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {

        //create new AFire Agents in the positions contained in the cross pattern returned by Utils.PatternCross()
        foreach (Vector2Int pos in Utils.PatternCross(states[1], position,g,new List<string> { "Agent_Strong_Wall" }, new List<string> { "Agent_Weak_Wall" }))
        {
            Vector2Int realPos = Utils.GetRealPos(position, pos, g.width, g.height);
            PutAgentOnGrid(realPos, new AFire(new List<int> { 0 }, realPos.x, realPos.y, this), g);
        }
    }
}
