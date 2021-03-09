using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeAgent : Agent
{
    public LifeAgent(int state, int x, int y) {
        this.state = state;
        this.position = new Vector2Int(x,y);
        this.type = 1;
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

        this.constant_sensors = new Vector2Int[] {};
    }

    public override void UpdateAgent(Agent[] sensors)
    {
        int sum=0;
        foreach(Agent sensor in sensors)
        {
            if (sensor.state == 1) sum++;
        }

        if (state == 1)
        {
            if (sum < 2 || sum > 3) state = 0;
            else state = 1;
        }
        else
        {
            if (sum == 3) state = 1;
            else state = 0;
        }
    }

}
