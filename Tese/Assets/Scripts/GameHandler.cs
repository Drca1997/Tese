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

    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid(width, width, new Vector3(origin[0], origin[1], origin[2]), cellSize,randomizeMap);
        grid.DisplayGrid();
        grid.DebugPrintGrid();
        GraphicsSetup();
        bombs = new List<Bomb>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckBombs();
            UpdateGraphics();
        }

        
        
    }

    void GraphicsSetup()
    {
        spriteRenderers = new List<GameObject>();
        for (int i = 0; i < grid.Array.GetLength(0); i++)
        {
            for (int j = 0; j < grid.Array.GetLength(1); j++)
            {
                spriteRenderers.Add(Utils.CreateSpriteRenderer(grid.GetWorldPosition(i, j), sprites[grid.Array[i,j]], cellSize));
            }
        }
    }

    void UpdateGraphics()
    {
        for (int i=0; i < grid.Array.GetLength(0); i++)
        {
            for (int j =0; j < grid.Array.GetLength(1); j++)
            {

                SpriteRenderer sR = spriteRenderers[i * grid.Array.GetLength(1) + j].GetComponent<SpriteRenderer>();
                if (sR == null)
                {
                    Debug.Log("ERRO: TILE NAO ENCONTRADA!");
                }
                else
                {
                    sR.sprite = sprites[grid.Array[i, j]];
                }
            }
        }
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
