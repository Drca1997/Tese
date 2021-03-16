using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private int [] origin;
    [SerializeField]
    private int cellSize;
    [SerializeField]
    private bool randomizeMap;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private int bombRadius; //inutil por agora, ate que faça logica da bomba modular para suportar diferentes radios
    private Grid grid;
    private List <GameObject> spriteRenderers;
    private List<Bomb> bombs;
    private GraphicsManager graphicsManager;

    private List<GameObject> agents;

    private void Awake()
    {
        agents = GameObject.FindGameObjectsWithTag("Agent").ToList();
    }
    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid(width, width, new Vector3(origin[0], origin[1], origin[2]), cellSize,randomizeMap);
        grid.DisplayGrid(false);
        //grid.DebugPrintGrid();
        bombs = new List<Bomb>();
        graphicsManager = new GraphicsManager(grid, sprites);
        AgentsSetup();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            ProcessAgentsActions();
            CheckBombs();
            graphicsManager.UpdateGraphics();
            CheckWinners();
        }
    }

    void ProcessAgentsActions()
    {
        for (int i=0; i < agents.Count; i++) 
        {
            
            BaseAgent agent = agents[i].GetComponent<BaseAgent>();
            int action = agents[i].GetComponent<IDecisionRequester>().RequestDecision();
            switch (action)
            {
                case 0: //move up
                    if (grid.Array[agent.X, agent.Y] == 5)
                        grid.Array[agent.X, agent.Y] = 4;
                    else
                        grid.Array[agent.X, agent.Y] = 1;

                    agent.Y += 1;
                    grid.Array[agent.X, agent.Y] = 0;
                    break;
                case 1: //move down
                    if (grid.Array[agent.X, agent.Y] == 5)
                        grid.Array[agent.X, agent.Y] = 4;
                    else
                        grid.Array[agent.X, agent.Y] = 1;

                    agent.Y -= 1;
                    grid.Array[agent.X, agent.Y] = 0;
                    break;
                case 2: //move west
                    if (grid.Array[agent.X, agent.Y] == 5)
                        grid.Array[agent.X, agent.Y] = 4;
                    else
                        grid.Array[agent.X, agent.Y] = 1;

                    agent.X -= 1;
                    grid.Array[agent.X, agent.Y] = 0;
                    break;

                case 3: //move east
                    if (grid.Array[agent.X, agent.Y] == 5)
                        grid.Array[agent.X, agent.Y] = 4;
                    else
                        grid.Array[agent.X, agent.Y] = 1;

                    agent.X += 1;
                    grid.Array[agent.X, agent.Y] = 0;
                    break;
                case 4: //plant bomb

                    Bomb bomba = new Bomb(grid, agent.X,
                        agent.Y, ref agent);
                    bombs.Add(bomba);
                    grid.Array[agent.X, agent.Y] = 5;
                    break;
                case 5: //do nothing
                    break;
                default:
                    break;
            }  
        }
       
    }

    void CheckBombs()
    {
        foreach (Bomb bomba in bombs.ToList())
        {
            bomba.Countdown -= 1;
            if (bomba.Countdown == 0) //bomba explode
            {
                Debug.Log("BOMBA EXPLODIU");
                List<int[]> affectedTiles = bomba.CheckBombRadius();
                foreach (int[] tile in affectedTiles)
                {
                    //se tile é um agente ou bloco explodivel
                    if (grid.Array[tile[0], tile[1]] == 0) 
                    {
                        Debug.Log("AGENTE MORREU");
                        GameObject deadAgent = GetAgent(tile[0], tile[1]);
                        agents.Remove(deadAgent);
                        Destroy(deadAgent);
                        bomba.Agent.PlantedBomb = false;
                        grid.Array[tile[0], tile[1]] = 1;
                    }
                    else if(grid.Array[tile[0], tile[1]] == 2)
                    {
                        bomba.Agent.PlantedBomb = false;
                        grid.Array[tile[0], tile[1]] = 1;
                    }

                }
                
                grid.Array[bomba.X, bomba.Y] = 1;
            }
            
        }
        
    }
    
    private void AgentsSetup()
    {
        int conta = 0;
       for (int i=0; i < grid.Array.GetLength(0); i++)
        {
            for (int j=0; j < grid.Array.GetLength(1); j++)
            {
                if (grid.Array[i,j] == 0)
                {
                    //depois ter o cuidado de verificar se "conta" nao excede indices da lista
                    agents[conta].GetComponent<IDecisionRequester>().GetWorld(grid, i, j);
                    conta += 1;
                }
            }
        }
    }

    private void CheckWinners()
    {
        if (agents.Count == 1)
        {
            Debug.Log(agents[0].name + " é o vencedor!");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        
    }

    private GameObject GetAgent(int x, int y)
    {
        foreach(GameObject agent in agents.ToList())
        {
            if (agent.GetComponent<BaseAgent>().X == x && agent.GetComponent<BaseAgent>().Y == y){
                return agent;
            }
        }
        return null;
    }
}
