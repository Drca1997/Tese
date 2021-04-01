using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Abstract class used as a basis for all agent types
public abstract class Agent 
{
    //An identifiying string for the agent type 
    public string typeName;

    //Agent's position on the grid
    public Vector2Int position;

    //"Sensors" are the positions on the grid whose contents(i.e. other Agents) the Agent may use as input on its update rules
    //List of positions relative to this Agent's position that are used as sensors
    public List<Vector2Int> relative_sensors = new List<Vector2Int> { };
    //List of fixed grid positions that this Agent uses as Sensors 
    public List<Vector2Int> constant_sensors = new List<Vector2Int> { };

    //List of ints of variable size that represent the internal states of the Agent
    //These states may be used as input for the Agent's update rules and/or changed acording to them
    public List<int> states = new List<int> { };

    //A boolean that indicates wether the Agent is in the grid (i.e. if it exists in the simulation and should be updated)
    public bool exists = true;

    //A list of strings that indicate what other types of Agents this Agent cannot coexist in the same grid position with
    public List<string> colliderTypes = new List<string> {};


    //Receives Grid (g), int (step_stage), and System.Random (prng)
    //The function that each Agent type must implement
    //In it, an action to be performed is chosen acording to the input and update rules of the Agent
    //As input, the update rules may use the state of the grid as seen in the Agent's sensors, or the Agent's states
    //An action may be modifying the grid by creating, modifying or deleting this or other Agents
    //If updating the whole grid one time step requires more than one update in which the Agents have to use different rules, the step_stage int may be used 
    public abstract void UpdateAgent(Grid g, int step_stage, System.Random prng);

    //Receives Grid (grid)
    //Returns a list of all Agents contained in the sensor positions on the grid
    public List<Agent> GetSensors(Grid grid)
    {
        List<Agent> agentSensors = new List<Agent> { };

        //List of all the positions of the grid used by the Agent as sensors
        List<Vector2Int> total_sensors = new List<Vector2Int> { };

        //Add the relative position sensors to the list of total sensors
        foreach (Vector2Int sensorPos in relative_sensors)
        {
            //The real position is added to the total sensors list
            Vector2Int realPos = GetRealPos(position, sensorPos, grid);
            total_sensors.Add(realPos);
        }

        //Add the fixed position sensors to the list of total sensors
        total_sensors.AddRange(constant_sensors);

        //remove duplicate positions
        total_sensors = new HashSet<Vector2Int>(total_sensors).ToList();

        foreach (Vector2Int sensorPos in total_sensors)
        {
            //Each Agent on the sensor position is added to the list
            foreach (Agent a in grid.agentGrid[sensorPos.x, sensorPos.y])
            {
                agentSensors.Add(a);
            }
        }

        return agentSensors;
    }

    //Receives Vector2Int (agentPos), Vector2Int (sensorPos), and Grid (grid)
    //Returns a Vector2Int
    //The grid loops horizontally and vertically such as the cells at one extreme have the cells at the other extreme as neighbors
    //This function receives a relative position to the Agent and returns the real position on the grid 
    public Vector2Int GetRealPos(Vector2Int agentPos, Vector2Int sensorPos, Grid grid)
    {
        Vector2Int realPos = agentPos + sensorPos;

        //Constraining the position on both axis
        realPos.x = Utils.LoopInt(0, grid.width, realPos.x);
        realPos.y = Utils.LoopInt(0, grid.height, realPos.y);
        
        return realPos;
    }

    //Receives Vector2Int (gridPos), Grid (grid), List<string> (colliderTypeList)
    //Returns bool
    //Function used to check if a given position on the grid contains an Agent with a typeName component contained in the given colliderTypeList
    public bool CollisionCheck(Vector2Int gridPos, Grid grid, List<string> colliderTypeList)
    {
        foreach(Agent a in grid.agentGrid[gridPos.x, gridPos.y])
        {
            if (colliderTypeList.Contains(a.typeName)) return true;
        }
        return false;
    }

    //Receives Vector2Int (newAgentPos), Agent (newAgent), Grid (grid)
    //Returns bool
    //Adds the given new Agent to the grid on the given position, returning true if it was successful and false otherwise
    public bool PutAgentOnGrid(Vector2Int newAgentPos, Agent newAgent, Grid grid)
    {
        //check for collisions
        if (!CollisionCheck(newAgentPos, grid, newAgent.colliderTypes))
        {
            newAgent.position = newAgentPos;
            grid.agentGrid[newAgentPos.x, newAgentPos.y].Add(newAgent);
            return true;
        }
        //returns false if collisions didn't permit adding the agent to the given position on the grid
        return false;
    }

    //Receives Agent (agentToRemove) and Grid (grid)
    //Removes the given Agent from its position on the grid without turning its "exists" component to false
    public void RemoveAgentOffGrid(Agent agentToRemove, Grid grid)
    {
        grid.agentGrid[agentToRemove.position.x, agentToRemove.position.y].Remove(agentToRemove);
    }

    //Receives Agent (agentToEliminate) and Grid (grid)
    //Eliminates the given Agent by calling the RemoveAgentOffGrid function and turning its "exists" component to false
    public void EliminateAgent(Agent agentToEliminate, Grid grid)
    {
        agentToEliminate.exists = false;
        RemoveAgentOffGrid(agentToEliminate, grid);
    }

    //Receives Vector2Int (newAgentPos), Agent (agentToMove), and Grid (grid)
    //Returns bool
    //Moves the given Agent from its position on the grid to a new, given, one 
    //Returns true if the operation was successful and false otherwise
    public bool MoveAgent(Vector2Int newAgentPos, Agent agentToMove, Grid grid)
    {
        //check for collisions
        if (!CollisionCheck(newAgentPos, grid, agentToMove.colliderTypes))
        {
            RemoveAgentOffGrid(agentToMove, grid);
            PutAgentOnGrid(newAgentPos, agentToMove, grid);
            return true;
        }
        //returns false if collisions didn't permit moving the agent to the given position on the grid
        return false;
    }

}