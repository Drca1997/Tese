using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABushman : GameAgent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public ABushman(List<int> states, int x, int y, IUpdate updateInterface)
    {
        //This agent cannot be placed in the same position of the agentGrid with the Agent types on this list
        this.colliderTypes.Add("Agent_Weak_Wall");
        this.colliderTypes.Add("Agent_Strong_Wall");
        this.colliderTypes.Add("Agent_Bomb");
        this.colliderTypes.Add("Agent_Bomberman");
        this.colliderTypes.Add("Agent_Bushman");
        this.colliderTypes.Add("Player_Bomberman");
        this.colliderTypes.Add("Malaquias_Bomberman");

        //timer until next bush and bush cooldown
        this.states = new List<int> { 0, 10 };
        this.position = new Vector2Int(x, y);
        this.typeName = "Agent_Bushman";
        this.updateInterface = updateInterface;

        this.relative_sensors = new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        this.constant_sensors = new List<Vector2Int> { };
    }

    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        if (states[0] > 0) states[0]--;

        //0-up 1-down 2-left 3-right
        int[] possible_move = new int[4];
        bool bush = false;
        bool danger = false;
        List<GameAgent> sensors = GetSensors(g);
        foreach (GameAgent a in sensors)
        {
            if (string.Compare(a.typeName, "Agent_Bomb") == 0 || string.Compare(a.typeName, "Agent_Fire") == 0)
            {
                danger = true;
                if (a.position.y > position.y) possible_move[0] = 2;
                else if (a.position.y < position.y) possible_move[1] = 2;
                else if (a.position.x < position.y) possible_move[2] = 2;
                else if (a.position.y > position.y) possible_move[3] = 2;
            }
            if (string.Compare(a.typeName, "Agent_Weak_Wall") == 0 || string.Compare(a.typeName, "Agent_Strong_Wall") == 0)
            {
                if (a.position.y > position.y) possible_move[0] = 1;
                else if (a.position.y < position.y) possible_move[1] = 1;
                else if (a.position.x < position.y) possible_move[2] = 1;
                else if (a.position.y > position.y) possible_move[3] = 1;
            }
            if (prng.Next(0, 100)<20)
            {
                bush = true;
            }
        }
        //see bomb in way - run
        //default - walk
        if (danger)
        {
            Vector2Int newPosition = position;
            if (possible_move[0] == 0) newPosition.y = Utils.LoopInt(0, g.height, newPosition.y + 1);
            else if (possible_move[1] == 0) newPosition.y = Utils.LoopInt(0, g.height, newPosition.y - 1);
            else if (possible_move[2] == 0) newPosition.x = Utils.LoopInt(0, g.width, newPosition.x - 1);
            else if (possible_move[3] == 0) newPosition.x = Utils.LoopInt(0, g.width, newPosition.x + 1);
            MoveAgent(newPosition, this, g);
            return;
        }
        if (bush && states[0] == 0)
        {
            PutAgentOnGrid(position, new ABush(new List<int> { 50, 50 }, position.x, position.y), g);
            states[0] = states[1];
            return;
        }
        List<Vector2Int> possibleNewPositions = new List<Vector2Int>();
        if (possible_move[0] == 0) possibleNewPositions.Add(new Vector2Int(0, 1));
        if (possible_move[1] == 0) possibleNewPositions.Add(new Vector2Int(0, -1));
        if (possible_move[2] == 0) possibleNewPositions.Add(new Vector2Int(-1, 0));
        if (possible_move[3] == 0) possibleNewPositions.Add(new Vector2Int(1, 0));

        //Debug.Log(possibleNewPositions.Count);
        if (possibleNewPositions.Count != 0)
        {
            Vector2Int newPosition = Utils.GetRealPos(position, possibleNewPositions[prng.Next(0, possibleNewPositions.Count)], g.width, g.height);
            MoveAgent(newPosition, this, g);
        }

    }

    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {
        updateInterface.AgentCall(this, g, prng);
    }
}
