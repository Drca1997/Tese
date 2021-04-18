using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SyntheticBombermanPlayer : Agent
{
    //se plantou uma bomba e esta ainda não explodiu
    private bool plantedBomb;

    //representação do Grid que os jogadores sintéticos usam
    private int[,] gridArray;

    public enum Tile
    {
        Player, PlayerEnemy, AIEnemy, Walkable, Explodable, Unsurpassable, Bomb, Fire, 
        PlayerNBomb, PlayerEnemyNBomb, AIEnemyNBomb, 
        FireNExplodable, FireNPlayer, FireNPlayerEnemy, FireNAIEnemy, FireNBomb,
        FireNBombNPlayer, FireNBombNPlayerEnemy, FireNBombNAIEnemy,
        Unknown
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
        Debug.Log("UPDATING AGENT");
        gridArray = ConvertGrid(g);
        ProcessAction(g, RequestDecision());

    }

    #region ConvertGrid
    private int[,] ConvertGrid(Grid g)
    {
        List<int>[,] agentGrid = g.ConvertAgentGrid();
        int[,] array = new int[g.width, g.height];
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
                        array[i, j] = agentGrid[i,j][0];
                        break;
                    case 2:
                        array[i, j] = DetermineTwoAgentsTileConfig(agentGrid[i,j]);
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
    private int DetermineTwoAgentsTileConfig(List<int> agentsInTile)
    {
        if (agentsInTile.Contains((int)Tile.Bomb))
        {
            return DetermineBombTile(agentsInTile);
        }
        else if (agentsInTile.Contains((int)Tile.Fire)){
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
            if (agentsInTile.Contains((int)Tile.Fire)){

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

    public void ProcessAction(Grid g, int action)
    {
        Vector2Int newPosition = position;

        //Calculate the new position/Create a new Abomb Agent acording to the input
        switch (action)
        {
            case 0:
                newPosition.y = Utils.LoopInt(0, g.height, newPosition.y + 1);
                //Move the Agent
                MoveAgent(newPosition, this, g);
                break;
            case 1:
                newPosition.y = Utils.LoopInt(0, g.height, newPosition.y - 1);
                //Move the Agent
                MoveAgent(newPosition, this, g);
                break;
            case 2:
                newPosition.x = Utils.LoopInt(0, g.width, newPosition.x - 1);
                //Move the Agent
                MoveAgent(newPosition, this, g);
                break;
            case 3:
                newPosition.x = Utils.LoopInt(0, g.width, newPosition.x + 1);
                //Move the Agent
                MoveAgent(newPosition, this, g);
                break;
            case 4:
                PutAgentOnGrid(position, new ABomb(new List<int> { 3 }, position.x, position.y, updateInterface), g);
                break;
        }


    }

    //usa para algo que querias que o agente faça ao ser removido da grid
    //de momento meti codigo para o agente avisar a interface de update que "morreu", para se saber quando a simulação deve ser parada
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {
        //na função AgentCall a interface vai lidar com decrementar a sua variável que indica o numero de jogadores
        updateInterface.AgentCall(this, g, prng);
    }

    public abstract int RequestDecision();

}
