using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static TextMesh CreateText(string text, Transform parent, Vector3 position, Vector3 scale, Color color, int fontsize, TextAnchor anchor, TextAlignment alignment)
    {
        GameObject obj = new GameObject("Text", typeof(TextMesh));

        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        TextMesh label = obj.GetComponent<TextMesh>();
        label.text = text;
        label.color = color;
        label.fontSize = fontsize;
        label.anchor = anchor;
        label.alignment = alignment;
        return label;
    }

    public static GameObject CreateSpriteRenderer(Vector3 worldPosition, Sprite sprite, float cellSize)
    {
        GameObject obj = new GameObject();
        obj.transform.position = worldPosition + new Vector3(cellSize, cellSize) * 0.5f;
        obj.AddComponent<SpriteRenderer>();
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        return obj;
    }

    public static bool IsTileWalkable(Grid grid, int x, int y) {
        if (grid.Array[x, y] == 1)
        {
            return true;
        }
        return false;
    }

    public static string ActionToString(int action)
    {
        switch (action)
        {
            case 0:
                return "moveu-se para cima";
            case 1:
                return "moveu-se para baixo";
            case 2:
                return "moveu-se para a esquerda";
            case 3:
                return "moveu-se para a direita";
            case 4:
                return "PLANTOU BOMBA";
            case 5:
                return "permaneceu no mesmo sítio";
               
        }
        return null;
    }

    public static bool IsValidAction(Grid grid, BaseAgent agent, int action)
    {
        switch (action)
        {
            case 0: //move up
                if (agent.Y + 1 < grid.Array.GetLength(1)) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X, agent.Y + 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 1: //move down
                if (agent.Y - 1 >= 0) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X, agent.Y - 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 2: //move left
                if (agent.X - 1 >= 0) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X - 1, agent.Y)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 3: //move right
                if (agent.X + 1 < grid.Array.GetLength(0)) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X + 1, agent.Y)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 4: //plant bomb
                if (!agent.PlantedBomb)
                {
                    agent.PlantedBomb = true;
                    return true;
                }

                return false;

            case 5: //do nothing
                return true;
            default:
                break;
        }
        return true;
    }

    /**
    public static List<int[]> GetTilesInBounds(Grid grid, int x, int y)
    {
        List<int[]> tiles = new List<int[]>();
        if (y + 1 < grid.Array.GetLength(1))
        {
            if (.IsTileAffected(x, y + 1))
            {
                tiles.Add(new int[] { x, y + 1 });
            }
            //se (x,y+2) nao está fora do mapa, e (x, y+1) nao é algo que tapou o radio da explosao
            if (y + 2 < grid.Array.GetLength(1) && grid.Array[x, y + 1] != 2 && grid.Array[x, y + 1] != 3)
            {
                if (IsTileAffected(x, y + 2))
                {
                    tTiles.Add(new int[] { x, y + 2 });
                }
            }
        }
        return tiles;
    }*/
}
