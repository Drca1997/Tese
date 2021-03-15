using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid(width, width, new Vector3(origin[0], origin[1], origin[2]), cellSize,randomizeMap);
        grid.DisplayGrid();
        grid.DebugPrintGrid();
        bombs = new List<Bomb>();
        graphicsManager = new GraphicsManager(grid, sprites);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            ProcessAgentsActions();
            CheckBombs();
            graphicsManager.UpdateGraphics();
        }
    }

    void ProcessAgentsActions()
    {

    }

    void CheckBombs()
    {
        foreach (Bomb bomba in bombs)
        {
            if (bomba.Countdown == 0) //bomba explode
            {
                List<int[]> affectedTiles = bomba.CheckBombRadius();
                foreach (int[] tile in affectedTiles)
                {
                    //se tile é um agente ou bloco explodivel
                    if (grid.Array[tile[0], tile[1]] == 0 || grid.Array[tile[0], tile[1]] == 2)
                    {

                        grid.Array[tile[0], tile[1]] = 1;
                    }

                }
                bombs.Remove(bomba);
            }
            
        }
    }
    
}
