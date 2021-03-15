using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils

{
    public static Vector3 GetWorldPosition(int x, int y, float cellSize)
    {
        return new Vector3(x, y) * cellSize;
    }

    public static void CreateText(string text, Vector3 position)
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
    public static GameObject CreateSquare(Color color, string name, Vector3 position, float cellSize)
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

    public static List<Agent> PutAgentsInList(Agent[,] agents)
    {
        List<Agent> listAgents = new List<Agent>();
        for (int x = 0; x < agents.GetLength(0); x++)
        {
            for (int y = 0; y < agents.GetLength(1); y++)
            {
                listAgents.Add(agents[x, y]);
            }
        }
        return listAgents;
    }

    //Implementação do Fisher–Yates shuffle para List<T>
    //https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm

    public static void Shuffle<T>(List<T> list, System.Random prng)
    {
        for(int i = list.Count-1; i>0; i--)
        {
            //number greater or equal to 0 and lesser than i
            int j= prng.Next(0, i+1);
            //swap list[i] and list[j]
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}