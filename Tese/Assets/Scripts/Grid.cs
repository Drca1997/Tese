using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid 
{
    private int width;
    private int height;
    private float cellSize;
    public Agent[,] agentGrid;
    private GameObject[,] objectGrid;

    public Grid (int width, int height, float cellSize, Agent[,] agentGrid)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.agentGrid = agentGrid;
        this.objectGrid = new GameObject[width, height];

        InstantiateObjectGrid();
    }

    public void InstantiateObjectGrid()
    {
        for (int i = 0; i < agentGrid.GetLength(0); i++)
        {
            for (int j = 0; j < agentGrid.GetLength(1); j++)
            {
                //CreateText(agentGrid[i, j].typeName, GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) / 2);
                objectGrid[i,j] = CreateSquare((agentGrid[i, j].state == 1) ? Color.black : Color.white, agentGrid[i, j].typeName, GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) / 2);  
            }
        }
    }

    public void UpdateObjectGrid()
    {
        for (int i = 0; i < agentGrid.GetLength(0); i++)
        {
            for (int j = 0; j < agentGrid.GetLength(1); j++)
            {
                objectGrid[i, j].GetComponent<SpriteRenderer>().color = (agentGrid[i, j].state == 1) ? Color.black : Color.white;
            }
        }
    }

    private Vector3 GetWorldPosition (int x, int y)
    {
        return new Vector3(x, y) * cellSize;
    }

    private void CreateText(string text, Vector3 position)
    {
        GameObject gameObject = new GameObject("Texto_Fixolas", typeof(TextMesh));
        Transform trans = gameObject.transform;
        trans.localPosition = position;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 20;
        textMesh.color = Color.black;
        textMesh.anchor = TextAnchor.MiddleCenter;
    }
    private GameObject CreateSquare(Color color, string name, Vector3 position)
    {  
        GameObject gameObject = new GameObject(name,typeof(SpriteRenderer));
        Transform trans = gameObject.transform;
        trans.localPosition = position;
        trans.localScale = new Vector3(cellSize, cellSize);
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.sprite = Resources.Load<Sprite>("Textures/um_pixel");
        sprite.color = color;
        return gameObject;
    }
}
