using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//IVisualize for the Game of Life with two agent types game scenario with representation for:
//LifeAgentAlive Agents
//LifeAgentDead Agents
//RandomMoveAgent Agents
//PlayerMovementAgent Agents
public class GameOfLife2Visualize : MonoBehaviour, IVisualize
{
    //If more than one Agent is in a position of the agentGrid, this priority list will dictate which one shall be represented in the objectGrid
    string[] priorityList = new string[] { "Player_Movement_Agent", "Random_Move_Agent", "Live_Agent_Alive", "Live_Agent_Dead" };

    //Receives the Grid object as a parameter
    //The GameObjects contained in the objectGrid component of the Grid object are updated in order to indicate what agent types are dontained in that same position on the agentGrid
    //This function is responsible for defining priorities of representation for different agent types
    public void VisualizeGrid(Grid grid)
    {
        //Going through every position on the agentGrid to update the respective one on the objectGrid
        for (int i = 0; i < grid.agentGrid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.agentGrid.GetLength(1); j++)
            {
                //returns the highest priority Agent from the i j position on the agentGrid
                GameAgent a = Utils.GetAgent(grid.agentGrid[i, j], priorityList);

                //if there aren't any agents on this position
                if (a == null)
                {
                    grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.white;
                    grid.objectGrid[i, j].name = "Empty";
                }
                //if there is at least one agent on this position
                else
                {
                    //Each type of agent has its own representation
                    grid.objectGrid[i, j].name = a.typeName;
                    switch (a.typeName)
                    {
                        case "Player_Movement_Agent":
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.green;
                            break;
                        case "Random_Move_Agent":
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.red;
                            break;
                        case "Live_Agent_Alive":
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.black;
                            break;
                        case "Live_Agent_Dead":
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.white;
                            break;
                        default:
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.magenta;
                            break;
                    }
                }
            }
        }
    }
}