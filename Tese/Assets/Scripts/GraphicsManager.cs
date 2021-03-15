using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager
{

    Grid grid;
    Sprite[] sprites;
    List<GameObject> spriteRenderers;

    public GraphicsManager(Grid grid, Sprite [] sprites)
    {
        this.grid = grid;
        this.sprites = sprites;
        GraphicsSetup();
    }

    private void GraphicsSetup()
    {
        spriteRenderers = new List<GameObject>();
        for (int i = 0; i < grid.Array.GetLength(0); i++)
        {
            for (int j = 0; j < grid.Array.GetLength(1); j++)
            {
                spriteRenderers.Add(Utils.CreateSpriteRenderer(grid.GetWorldPosition(i, j), sprites[grid.Array[i, j]], grid.CellSize));
            }
        }
    }

    public void UpdateGraphics()
    {
        for (int i = 0; i < grid.Array.GetLength(0); i++)
        {
            for (int j = 0; j < grid.Array.GetLength(1); j++)
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
}
