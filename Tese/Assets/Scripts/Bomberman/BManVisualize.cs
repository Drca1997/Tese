using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//IVisualize for the Bomberman game scenario with representation for:
//Player Bomberman Agents
//Bomb Agents
//Fire Agents
//Weak Wall Agents

public class BManVisualize : MonoBehaviour, IVisualize
{
    //If more than one Agent is in a position of the agentGrid, this priority list will dictate which one shall be represented in the objectGrid
    string[] priorityList = new string[] { "Malaquias_Bomberman", "Player_Bomberman", "Agent_Bomberman", "Agent_Bomb", "Agent_Fire", "Agent_Strong_Wall", "Agent_Weak_Wall" };

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
                    switch (a.typeName) {
                        case "Malaquias_Bomberman":
                            if (a.GetType() == typeof(MLSyntheticPlayer))
                            {
                                MLSyntheticPlayer player = (MLSyntheticPlayer)a;
                                if (player.TeamID == 0)
                                {
                                    //grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.cyan;
                                    grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.blue;
                                    
                                    
                                }
                                else
                                {
                                    //grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = new Color(0 / 255f, 102 / 255f, 102 / 255f);
                                    grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 165 / 255f, 0 / 255f); //Laranja

                                }
                                
                            }
                            else if(a.GetType() == typeof(PlanningSyntheticPlayer))
                            {
                                //grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 165 / 255f, 0 / 255f);
                                
                                grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = new Color(255 / 225f, 20 / 225f, 147 / 255f); //Rosa


                            }
                            else if (a.GetType() == typeof(RandomSyntheticPlayer))
                            {
                                //grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = new Color(255/225f, 20/225f, 147/255f);
                                
                               
                            }
                            else if (a.GetType() == typeof(IdleSyntheticPlayer))
                            {
                                grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = new Color(128 / 225f, 0 / 225f, 128 / 255f);
                            }
                            
                            break;
                        case "Player_Bomberman":
                            //grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.blue;
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.green;
                            
                            break;
                        case "Agent_Bomberman":
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.green;
                            break;
                        case "Agent_Bomb":
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.red;
                            break;
                        case "Agent_Fire":
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.yellow;
                            break;
                        case "Agent_Strong_Wall":
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.black;
                            break;
                        case "Agent_Weak_Wall":
                            grid.objectGrid[i, j].GetComponent<SpriteRenderer>().color = Color.grey;
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
