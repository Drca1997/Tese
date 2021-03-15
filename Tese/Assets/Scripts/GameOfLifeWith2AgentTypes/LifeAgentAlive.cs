using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeAgentAlive : Agent
{
    public LifeAgentAlive(List<int> states, int x, int y)
    {

        //states[0] - alive neighours 
        //states[1] - alive/dead
        this.states = states;
        this.position = new Vector2Int(x, y);
        this.typeName = "Live_Agent";
        this.relative_sensors = new Vector2Int[] {
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)};

        this.constant_sensors = new Vector2Int[] { };
    }

    public override Grid UpdateAgent(Grid g, int step_stage)
    {
        switch (step_stage)
        {
            //contar numero de vizinhos vivios
            case 0:
                Agent[] sensors = GetSensors(g);
                states[0] = 0;
                foreach (Agent sensor in sensors)
                {
                    if (sensor.states[1] == 1) states[0]++;
                }
                break;

            //mudar de estado de acordo com o numero de vizinhos
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
        return g;

    }

}