using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SyntheticBombermanPlayer : GameAgentPlayer
{
    //se plantou uma bomba e esta ainda não explodiu
    protected bool plantedBomb;
    protected ABomb bomb;

    //representação do Grid que os jogadores sintéticos usam
    protected int[,] gridArray;

    public enum Tile
    {
        Player, PlayerEnemy, AIEnemy, Walkable, Explodable, Unsurpassable, Bomb, Fire, 
        PlayerNBomb, PlayerEnemyNBomb, AIEnemyNBomb, 
        FireNExplodable, FireNPlayer, FireNPlayerEnemy, FireNAIEnemy, FireNBomb,
        FireNBombNPlayer, FireNBombNPlayerEnemy, FireNBombNAIEnemy
    }

    public enum Action
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        PlantBomb,
        DoNothing
    }

    public bool PlantedBomb { get => plantedBomb; set => plantedBomb = value; }
    public int[,] GridArray { get => gridArray; set => gridArray = value; }

    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public SyntheticBombermanPlayer(List<int> states, int x, int y, IUpdate updateInterface)
    {
        //This agent cannot be placed in the same position of the agentGrid with the Agent types on this list
        this.colliderTypes.Add("Agent_Weak_Wall");
        this.colliderTypes.Add("Agent_Strong_Wall");
        this.colliderTypes.Add("Agent_Bomb");
        this.colliderTypes.Add("Player_Bomberman");
        this.colliderTypes.Add("Agent_Bomberman");
        this.colliderTypes.Add("Malaquias_Bomberman");

        this.states = states;
        this.position = new Vector2Int(x, y);
        this.typeName = "Malaquias_Bomberman";
        this.updateInterface = updateInterface;

        plantedBomb = false;
    }

    //chamado para atualizar o agente em cada time step
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        //podes usar g.agentGrid para obter a matriz de List<Agent>
        //g.ConvertAgentGrid() devolve-te uma List<int>[,] correspondente
        //tem em conta que algumas das listas podem estar vazias ou ter multiplos elementos, visto que há sitios na grelha sem ou com multiplos agentes
        //Os inteiros correspondem aos indices que os tipos de agente ocupam em g.agentTypes - podes modificá-los na interface de setup, na criação da nova Grid

        //Para mover o agente e criar uma bomba podes consultar o meu codigo em PBomberman.cs na função Logic
        
        //Debug.Log("UPDATING AGENT in " + position.x + ", " + position.y);
        HasBomb();
        gridArray = ConvertGrid(g);
        ProcessAction(g, TakeAction());
        
    }

    protected void HasBomb()
    {
        if (bomb != null)
        {
            plantedBomb = bomb.exists;
        }
        else
        {
            plantedBomb = false;
        }
    }


    #region ConvertGrid
    //Turns the game world into a grid representation, common to both synthetic players
    protected int[,] ConvertGrid(Grid g)
    {
        List<int>[,] agentGrid = g.ConvertAgentGrid();
        int[,] array = new int[g.width, g.height];
        DistinguishBetweenSyntheticPlayers(g, agentGrid);
        for (int i = 0; i < agentGrid.GetLength(0); i++)
        {
            for (int j = 0; j < agentGrid.GetLength(1); j++)
            {
                int agentCount = agentGrid[i, j].Count;
                switch (agentCount)
                {
                    case 0:
                        array[i, j] = (int)Tile.Walkable;
                        break;
                    case 1:
                        
                        if (agentGrid[i,j][0] == (int)Tile.PlayerEnemy)
                        {
                            array[i, j] = (int)Tile.AIEnemy; //diminuir numero de configuraçoes, juntar PlayerEnemy e AIEnemy
                        }
                        else
                        {
                            array[i, j] = agentGrid[i, j][0];
                        }
                       
                        break;
                    case 2:
                        array[i, j] = DetermineTwoAgentsTileConfig(agentGrid[i, j]);
                        break;
                    case 3:
                        array[i, j] = DetermineThreeAgentsTileConfig(agentGrid[i, j]);
                        break;
                    default:
                        array[i, j] = -1;
                        break;
                }

            }
        }
        return array;
    }

    private void DistinguishBetweenSyntheticPlayers(Grid grid, List<int>[,] agentGrid)
    {
        for (int i = 0; i < agentGrid.GetLength(0); i++)
        {
            for (int j = 0; j < agentGrid.GetLength(1); j++)
            {
                for (int agent = 0; agent < agentGrid[i, j].Count; agent++)
                {
                    if (agentGrid[i, j][agent] == 0)
                    {
                        foreach (GameAgent a in grid.agentGrid[i, j])
                        {
                            if (a.typeName.Equals("Malaquias_Bomberman"))
                            {
                                
                                      
                                if (a.position.x == position.x && a.position.y == position.y)
                                {
                                    agentGrid[i, j][agent] = (int)Tile.Player;
                                }
                                else
                                {
                                    agentGrid[i, j][agent] = (int)Tile.AIEnemy;
                                }
                            }  
                        }
                    }
                }
            }
        }
    }

  
    private int DetermineTwoAgentsTileConfig(List<int> agentsInTile)
    {
        if (agentsInTile.Contains((int)Tile.Bomb))
        {
            return DetermineBombTile(agentsInTile);
        }
        else if (agentsInTile.Contains((int)Tile.Fire))
        {
            return DetermineFireTile(agentsInTile);
        }
        return -1;
    }
    private int DetermineBombTile(List<int> agentsInTile)
    {
        if (agentsInTile.Contains((int)Tile.Player))
        {
            return (int)Tile.PlayerNBomb;
        }
        else if (agentsInTile.Contains((int)Tile.PlayerEnemy))
        {
            return (int)Tile.PlayerEnemyNBomb;
        }
        else if (agentsInTile.Contains((int)Tile.AIEnemy))
        {
            return (int)Tile.AIEnemyNBomb;
        }
        else if (agentsInTile.Contains((int)Tile.Fire))
        {
            return (int)Tile.FireNBomb;
        }
        return -1;
    }
    private int DetermineFireTile(List<int> agentsInTile)
    {
        if (agentsInTile.Contains((int)Tile.Player))
        {
            return (int)Tile.FireNPlayer;
        }
        else if (agentsInTile.Contains((int)Tile.PlayerEnemy))
        {
            return (int)Tile.FireNPlayerEnemy;
        }
        else if (agentsInTile.Contains((int)Tile.AIEnemy))
        {
            return (int)Tile.FireNAIEnemy;
        }
        else if (agentsInTile.Contains((int)Tile.Bomb))
        {
            return (int)Tile.FireNBomb;
        }
        else if (agentsInTile.Contains((int)Tile.Explodable))
        {
            return (int)Tile.FireNExplodable;
        }
        return -1;
    }
    private int DetermineThreeAgentsTileConfig(List<int> agentsInTile)
    {
        if (agentsInTile.Contains((int)Tile.Bomb))
        {
            if (agentsInTile.Contains((int)Tile.Fire))
            {

                if (agentsInTile.Contains((int)Tile.Player))
                {
                    return (int)Tile.FireNBombNPlayer;
                }
                else if (agentsInTile.Contains((int)Tile.PlayerEnemy))
                {
                    return (int)Tile.FireNBombNPlayerEnemy;
                }
                else if (agentsInTile.Contains((int)Tile.AIEnemy))
                {
                    return (int)Tile.FireNBombNAIEnemy;
                }
            }
        }
        return -1;
    }



    #endregion

    //Responsible for dealing with the game logic of executing actions in the game
    public void ProcessAction(Grid g, int action)
    {
        Vector2Int newPosition = position;
        //Calculate the new position/Create a new Abomb Agent acording to the input
        switch (action)
        {
            case (int)Action.MoveUp:
                UpdateTileInGrid(action, position.x, position.y);
                newPosition.y = Utils.LoopInt(0, g.height, newPosition.y + 1);
                //Move the Agent
                MoveAgent(newPosition, this, g);
                UpdateTileInGrid(action, position.x, position.y);
                break;
            case (int)Action.MoveDown:
                UpdateTileInGrid(action, position.x, position.y);
                newPosition.y = Utils.LoopInt(0, g.height, newPosition.y - 1);
                //Move the Agent
                MoveAgent(newPosition, this, g);
                UpdateTileInGrid(action, position.x, position.y);
                break;
            case (int)Action.MoveLeft:
                UpdateTileInGrid(action, position.x, position.y);
                newPosition.x = Utils.LoopInt(0, g.width, newPosition.x - 1);
                //Move the Agent
                MoveAgent(newPosition, this, g);
                UpdateTileInGrid(action, position.x, position.y);
                break;
            case (int)Action.MoveRight:
                UpdateTileInGrid(action, position.x, position.y);
                newPosition.x = Utils.LoopInt(0, g.width, newPosition.x + 1);
                //Move the Agent
                MoveAgent(newPosition, this, g);
                UpdateTileInGrid(action, position.x, position.y);
                break;
            case (int)Action.PlantBomb:
                bomb = new ABomb(new List<int> { 3, 2 }, position.x, position.y, this);
                PutAgentOnGrid(position, bomb, g);
                UpdateTileInGrid(action, position.x, position.y);
                break;
        }


    }

    private void UpdateTileInGrid(int action, int x, int y)
    {
        switch (action)
        {
            case (int)Action.MoveUp:
            case (int)Action.MoveDown:
            case (int)Action.MoveLeft:
            case (int)Action.MoveRight:
                UpdateTileAfterMovement(x, y);
                break;
            case (int)Action.PlantBomb:
                UpdateTileAfterPlantingBomb(x, y);
                break;
        }
    }

    private void UpdateTileAfterMovement(int x, int y)
    {
        switch(gridArray[x, y])
        {
            case (int)Tile.Player:
                gridArray[x, y] = (int)Tile.Walkable;
                break;
            case (int)Tile.PlayerNBomb:
                gridArray[x, y] = (int)Tile.Bomb;
                break;
            case (int)Tile.FireNBombNPlayer:
                gridArray[x, y] = (int)Tile.FireNBomb;
                break;
            
        }
    }

    private void UpdateTileAfterPlantingBomb(int x, int y)
    {
        switch (gridArray[x, y])
        {
            case (int)Tile.Player:
                gridArray[x, y] = (int)Tile.PlayerNBomb;
                break;
            case (int)Tile.FireNPlayer:
                gridArray[x, y] = (int)Tile.FireNBombNPlayer;
                break;
        }
    }

    public abstract int TakeAction();

    //Delay in executing an action. Only for used to record videos for the survey
    protected void ReactionTime()
    {
        float delay = 12f;
        float waitingTime = 0f;
        while (true)
        {
            Debug.Log("UPDATE");
            waitingTime += Time.deltaTime;
            if (waitingTime >= delay) 
            {

                waitingTime = 0f;
                break;
            }
        }
    }

}
