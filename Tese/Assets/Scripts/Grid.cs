using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid 
{
    public int width;
    public int height;
    public float cellSize;
    public Agent[,] agentGrid;
    public GameObject[,] objectGrid;

    public Grid (int width, int height, float cellSize, Agent[,] agentGrid, GameObject[,] objectGrid)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.agentGrid = agentGrid;
        this.objectGrid = objectGrid;
    }

    public Vector3 GetWorldPosition (int x, int y)
    {
        return new Vector3(x, y) * cellSize;
    }

    public void CreateText(string text, Vector3 position)
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
    public GameObject CreateSquare(Color color, string name, Vector3 position)
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
