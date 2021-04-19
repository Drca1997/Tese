using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//IUpdate Interface for the Game of Life game scenario with:
//An update loop with two steps as to coordenate the action of all agents to simulate synchronous action 
//and 3 states:
//Finished updating
//Updating Non-Player Agents
//Updating Player Agent
public class GameOfLifeUpdate : MonoBehaviour, IUpdate
{
    //Bollean that indicates if the update loop has ended
    public bool finishedLoop = true;

    //Indicates the current stage in the loop
    //each loop has 2 for this game scenario
    //in the first one, the Game of Life Agents update one of their internal states to the number of alive neighbors, and the remaining Agent types are update as normal
    //in the second,  the Game of Life Agents update to "dead" or "alive" acording with their internal states and rules
    //since in this model the Agents update asynchronously, to achieve the synchronous action that the Game of Live requiers, these two steps have to be separate
    public int loopNumber = 1;

    //Bollean that indicates if currently a Player Agent is being updated
    public bool updatingPlayer = false;

    //List of all Agents contained in the Grid 
    //Agents are updated in the order they appear in this list
    //The list is shuffled before the update cycle begins
    public List<GameAgent> randList;
    //The randList may not be fully looped through in one execution of UpdateGrid (if the player input is needed, for example)
    //A reference to the index of the current Agent on randList is stored
    public int index;


    public void SetupSimulation(Grid g, System.Random prng)
    {

    }


    //Receives the Grid object and a System.Random as a parameters
    //The agents contained in the agentGrid component of the Grid object are updated acording with the instrunctions in this function
    //This function is responsible for the order in which the agents are updated, as well as how to handle agents that require player input
    public void UpdateGrid(Grid grid, System.Random prng)
    {
        //If the last update cycle is over, and the right mouse button is pressed, then we start a new one
        if (finishedLoop && Input.GetMouseButtonDown(1))
        {
            finishedLoop = false;
            //The randlist is rebuilt (since in the last update cycle new Agents may have been added to the grid, or old Agents removed)
            randList = Utils.PutAgentsInList(grid.agentGrid);
            //randList is shuffled in order to update the Agents in a random order
            Utils.Shuffle<GameAgent>(randList, prng);
            //index is reinitialized
            index = 0;
            //The update loop starts on its first step
            loopNumber = 1;
        }

        //While the update cycle isn't over (i.e. there are still Agents that need to be updated)
        while (!finishedLoop)
        {
            //If currently we are updating a Player Agent 
            if (updatingPlayer)
            {
                GameAgentPlayer a = randList[index] as GameAgentPlayer;
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
                //Depending on the stage of the update loop, the Agents will be updated differently 
                switch (loopNumber)
                {
                    //All Agents are updated
                    case 1:
                        //Going through each Agent in randList
                        while (index < randList.Count)
                        {
                            //Since Agents can remove other Agents from the agentGrid when updated, if a previous Agent removed the current one in this 
                            //update cycle, it shouldn't be updated, i.e. affect the agentGrid
                            //The boolean component "exists" of the Agents indicates this
                            if (randList[index].exists)
                            {
                                //The current Agent is updated with the step_stage parameter set to 0, since this some of the Agents (Game of Life Agents) have different sets of rules for each update stage
                                randList[index].UpdateAgent(grid, 0, prng);

                                //If the current Agent that was updated is an AgentPlayer (i.e. controlled by a player) and has not finished updating in this frame,
                                //we enter the "Updating Player Agent" state and exit the function (we can't continue the update loop until this agent is fully updated)
                                if (randList[index] is GameAgentPlayer)
                                {
                                    GameAgentPlayer a = randList[index] as GameAgentPlayer;
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
                        //with the first stage of the update loop done, we move on to the second one
                        //after exeting the switch statement, the while loop will move us to the second case
                        loopNumber = 2;
                        index = 0;
                        break;

                    //Only the Game of Life Agents are updated, with a second set of rules
                    case 2:
                        //Going through each Agent in randList
                        while (index < randList.Count)
                        {
                            //RandomMoveAgents and Player Agents have already been fully updated in the first update loop stage
                            if (randList[index].exists && string.Compare(randList[index].typeName, "Random_Move_Agent") != 0 && !(randList[index] is GameAgentPlayer)) randList[index].UpdateAgent(grid, 1, prng);
                            index++;
                        }

                        //If there are no more Agents to update, then the update loop is over and the objectGrid may be updated to convey visually the new state of the agentGrid
                        finishedLoop = true;
                        grid.updated = true;
                        break;
                }
            }
        }
    }

    public void AgentCall(GameAgent agent, Grid grid, System.Random prng)
    {

    }
}
