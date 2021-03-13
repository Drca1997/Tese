using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLifeSetup : MonoBehaviour, ISetup
{
    public int width = 20;
    public int height = 10;
    public float cellSize = 10f;
    [Range(0, 100)]
    public int randomFillPercetn;
    public Grid SetupGrid(System.Random prng)
    {
        Agent[,] agentGrid = new LifeAgent[width, height];
        GameObject[,] objectGrid = new GameObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                agentGrid[x, y] = new LifeAgent(new List<int> { 0, (prng.Next(0, 100) < randomFillPercetn) ? 1 : 0 }, x, y);
                //CreateText(agentGrid[i, j].typeName, GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) / 2);
                objectGrid[x, y] = CreateSquare((agentGrid[x, y].states[1] == 1) ? Color.black : Color.white, agentGrid[x, y].typeName, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) / 2);
            }
        }
        Grid grid = new Grid(width, height, cellSize, agentGrid, objectGrid);
        return grid;
    }

    public Vector3 GetWorldPosition(int x, int y)
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
        GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
        Transform trans = gameObject.transform;
        trans.localPosition = position;
        trans.localScale = new Vector3(cellSize, cellSize);
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.sprite = Resources.Load<Sprite>("Textures/um_pixel");
        sprite.color = color;
        return gameObject;
    }
}
