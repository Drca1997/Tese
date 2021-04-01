using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    //Receives 3 int parameters: max, min, and x
    //Returns a new int contained between min and max (not including max) and related with x
    //Used manly to recalculate positions on the grid as if the cells on one extreme are neighbours of the ones on the other extreme
    //After value max-1 comes value min, and before value min comes value max-1
    public static int LoopInt(int min, int max, int x)
    {
        //if x is smaller than min, then (max - 1) - (min - 1) is added to it 
        //LoopInt is recursevly called once again in case x is not inbetween min and max-1 yet
        if (x < min)
        {
            x = x + (max - 1) - (min - 1);
            x = LoopInt(min, max, x);
        }

        //if x is greater than max, then (max - 1) + (min - 1) is subtracted to it 
        //LoopInt is recursevly called once again in case x is not inbetween min and max-1 yet
        if (x >= max)
        {
            x = x - (max - 1) + (min - 1);
            x = LoopInt(min, max, x);
        }

        //If the initial value x was already inbetween min and max-1, then it is returned unchanged
        return x;
    }

    //Receives a List<Agent> (aList) and a string (aType) as parametres
    //Returns the first Agent object on the aList that as an typeName component equal to aType 
    //otherwise, null is returned
    public static Agent AgentListContinesType(List<Agent> aList, string aType)
    {
        foreach(Agent a in aList)
        {
            if (string.Compare(a.typeName, aType) == 0) return a;
        }
        return null;
    }

    //Receives a string (text) and a Vector3 (position) as parameters
    //Creates a GameObject with a TextMesh component on the position given as parameter and with the given text
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

    //Receives a Color (color), a string (name), a Vector3 (position), and a float (cellSize) as parameters
    //Creates and returns a GameObject with a SpriteRenderer component on the position given as parameter
    //The GameObject has the scale, color, and name given as parameters  
    public static GameObject CreateSquare(Color color, string name, Vector3 position, float cellSize)
    {
        GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
        Transform trans = gameObject.transform;
        trans.localPosition = position;
        trans.localScale = new Vector3(cellSize, cellSize);
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        //"Textures/um_pixel" is a white image of 1x1 dimensions
        sprite.sprite = Resources.Load<Sprite>("Textures/um_pixel");

        sprite.color = color;
        return gameObject;
    }


    //Returns a List<Agent> containting all the Agent objects found in the List<Agent>[,] given has a parameter
    public static List<Agent> PutAgentsInList(List<Agent>[,] agents)
    {
        List<Agent> listAgents = new List<Agent> { };
        for (int x = 0; x < agents.GetLength(0); x++)
        {
            for (int y = 0; y < agents.GetLength(1); y++)
            {
                foreach (Agent a in agents[x, y])
                {
                    listAgents.Add(a);
                }
            }
        }
        return listAgents;
    }

    //Implementation of the Fisher–Yates shuffle for List<T>
    //https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm

    public static void Shuffle<T>(List<T> list, System.Random prng)
    {
        for(int i = list.Count-1; i>0; i--)
        {
            //number greater or equal to 0 and lesser than i
            int j= prng.Next(0, i+1);
            //swap list[i] and list[j]d
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    //Receives a List<Agent> (agentList) and a string[] (priorityList)
    //Returns the first Agent found on agentList with the typeName equal to the highest string on priorityList
    public static Agent GetAgent(List<Agent> agentList, string[] priorityList)
    {
        if (agentList.Count == 0) return null;
        else if (agentList.Count == 1) return agentList[0];
        else
        {
            foreach (string type in priorityList)
            {
                Agent a = Utils.AgentListContinesType(agentList, type);
                if (a != null) return a;
            }
        }
        return null;
    }
}