using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStrongWall2 : GameAgent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public AStrongWall2(List<int> states, int x, int y)
    {
        //states[0] delay entre growth
        //states[1] countdown to growth
        if (states.Count == 2)
        {
            this.states = states;
        }
        else this.states = new List<int> { 50, 50 };
        this.position = new Vector2Int(x, y);
        this.typeName = "Agent_Strong_Wall";

        this.relative_sensors = new List<Vector2Int> { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        this.constant_sensors = new List<Vector2Int> { };
    }

    //Receives Grid(g), int (step_stage), and System.Random(prng)
    //Dosen't do anything, its an unbreakable wall
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {

        if (states[0] != 0)
        {
            if (states[1] != 0) states[1] -= 1;
            else
            {
                
                states[1] = states[0];
                Utils.Shuffle<Vector2Int>(relative_sensors, prng);
                foreach (Vector2Int pos in relative_sensors)
                {
                    Vector2Int realPos = Utils.GetRealPos(position, pos, g.width, g.height);
                    if (g.agentGrid[realPos.x, realPos.y].Count == 0)
                    {
                        PutAgentOnGrid(realPos, new AWeakWall2(new List<int> { states[0], states[0] }, realPos.x, realPos.y), g);
                        break;
                    }
                }
            }
        }
    }

    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //Executed on the elimination of the Agent form the agentGrid
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {

    }
}
