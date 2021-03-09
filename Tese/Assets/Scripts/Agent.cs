using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent 
{
    public int type;
    public string typeName;
    public Vector2Int position;
    public Vector2Int[] relative_sensors;
    public Vector2Int[] constant_sensors;
    public int state;
   
    public virtual void UpdateAgent(Agent[] sensors) { }

}
