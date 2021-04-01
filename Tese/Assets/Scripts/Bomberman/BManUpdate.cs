using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//IUpdate Interface for the Bomberman game scenario with 3 states:
//Finished updating
//Updating Non-Player Agents
//Updating Player Agent
public class BManUpdate : MonoBehaviour, IUpdate
{
    //Bollean that indicates if the update loop has ended
    public bool finishedLoop = true;
    //Bollean that indicates if currently a Player Agent is being updated
    public bool updatingPlayer = false;

    //List of all Agents contained in the Grid 
    //Agents are updated in the order they appear in this list
    //The list is shuffled before the update cycle begins
    public List<Agent> randList;
    //The randList may not be fully looped through in one execution of UpdateGrid (if the player input is needed, for example)
    //A reference to the index of the current Agent on randList is stored
    public int index;

    //Receives the Grid object and a System.Random as a parameters
    //The agents contained in the agentGrid component of the Grid object are updated acording with the instrunctions in this function
    //This function is responsible for the order in which the agents are updated, as well as how to handle agents that require player input
    public void UpdateGrid(Grid grid, System.Random prng)
    {
        //If the last update cycle is over, then we start a new one
        if (finishedLoop)
        {
            finishedLoop = false;
            //The randlist is rebuilt (since in the last update cycle new Agents may have been added to the grid, or old Agents removed)
            randList = Utils.PutAgentsInList(grid.agentGrid);
            //randList is shuffled in order to update the Agents in a random order
            Utils.Shuffle<Agent>(randList, prng);
            //index is reinitialized
            index = 0;
        }
        
        //While the update cycle isn't over (i.e. there are still Agents that need to be updated)
        while (!finishedLoop)
        {
            //If currently we are updating a Player Agent 
            if (updatingPlayer)
            {
                AgentPlayer a = randList[index] as AgentPlayer;
                //If the Agent as been updated, we exit "Updating Player Agent" state back to "Updating Non-Player Agent" state
                if (a.updated)
                {
                    updatingPlayer = false;
                    index++;
                }
                //If the Agent is still updating (waiting player input) we exit the while loop and the function
                //The update cycle only continues when the current Player Agent is fully updated
                else
                {
                    break;
                }
            }
            //If we are in the "Updating Non-Player Agent" state
            if (!updatingPlayer)
            {
                //Going through each Agent in randList
                while (index < randList.Count)
                {
                    //Since Agents can remove other Agents from the agentGrid when updated, if a previous Agent removed the current one in this 
                    //update cycle, it shouldn't be updated, i.e. affect the agentGrid
                    //The boolean component "exists" of the Agents indicates this
                    if (randList[index].exists)
                    {
                        //The current Agent is updated with the step_stage parameter set to 0, however this Interface does not require multiple update stages
                        randList[index].UpdateAgent(grid, 0, prng);

                        //If the current Agent that was updated is an AgentPlayer (i.e. controlled by a player) and has not finished updating in this frame,
                        //we enter the "Updating Player Agent" state and exit the function (we can't continue the update loop until this agent is fully updated)
                        if (randList[index] is AgentPlayer)
                        {
                            AgentPlayer a = randList[index] as AgentPlayer;
                            if (!a.updated)
                            {
                                updatingPlayer = true;
                                //The objectGrid will be updated so that the player may see the current state of agentGrid (with all the modifications allready done on this update loop)
                                grid.updated = true;
                                return;
                            }
                        }
                    }
                    index++;
                }

                //If there are no more Agents to update, then the update loop is over and the objectGrid may be updated to convey visually the new state of the agentGrid
                finishedLoop = true;
                grid.updated = true;
            }
        }
    }

    
}
