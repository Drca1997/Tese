using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Interface related with setting up the grid of agents before the start of the simulation 
//Different ISetup Interfaces may use different types of agents and/or ways of organizing them in the grid
//Specific IUpdate or IVisualize Interfaces may be requiered in order to work with the same types of agents
public interface ISetup
{
    //Receives a System.Random as a parameter that may be used for randomization
    //Returns a Grid object, setted up for the start of the simulation
    //This function is responsible for the creation of the Grid object, the dimensions of the grid, and the initial distribution of agents in the agentGrid
    Grid SetupGrid(System.Random prng);
}

//Interface related with moving the simulation forward by updating the agents on the Grid object
//May or may not be designed for specific types of agent combinations
public interface IUpdate
{
    void SetupSimulation(Grid g, System.Random prng);

    //Receives the Grid object and a System.Random as a parameters
    //The agents contained in the agentGrid component of the Grid object are updated acording with the instrunctions in this function
    //This function is responsible for the order in which the agents are updated, as well as how to handle agents that require player input
    void UpdateGrid(Grid g, System.Random prng);

    void AgentCall(GameAgent agent, Grid grid, System.Random prng);

    public event EventHandler OnMLAgentWin;
}

//Interface related with updating the objectGrid component of the Grid object acording with the state of the agentGrid component
//The SpriteRenderer components of the GameObjects contained in the objectGrid are modified acording to which agents are in the same position on the agentGrid
//Different IVisualize Interfaces may be design for specific types of agent combinations
public interface IVisualize
{
    //Receives the Grid object as a parameter
    //The GameObjects contained in the objectGrid component of the Grid object are updated in order to indicate what agent types are dontained in that same position on the agentGrid
    //This function is responsible for defining priorities of representation for different agent types 
    void VisualizeGrid(Grid g);
}