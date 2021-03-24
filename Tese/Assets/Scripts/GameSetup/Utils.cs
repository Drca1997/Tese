using System;
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


    #region ASTAR
    public static List<GraphNode> AStar(Graph graph, GraphNode start, List<GraphNode> possibleGoals, Func<GraphNode, List<GraphNode>, double> heuristic)
    {
        List<GraphNode> expandableList = new List<GraphNode>{start}; //pega-se no primeiro nó
        List<GraphNode> visitedList = new List<GraphNode>();

        start.HCost = heuristic(start, possibleGoals);
        start.FCost = start.GCost + start.HCost;

        while (expandableList.Count != 0) //enquanto houver nós para serem visitados
        {
            Debug.Log("EXPANDABLE LIST:");
            DebugAStarList(expandableList);
            Debug.Log("VISITED LIST:");
            DebugAStarList(visitedList);

            GraphNode node = GetLowestFCostNode(expandableList);
            Debug.Log("SELECIONOU-SE NÓ " + node.Index);
            if (possibleGoals.Contains(node))  //verifica-se se é objetivo
            {
                Debug.Log("OBJETIVO ENCONTRADO");
                return GetPath(node);
                
            }
            //se nao for nó objetivo 
            Debug.Log("NÓ ADICIONADO Á LISTA DE VISITADOS");
            expandableList.Remove(node);
            visitedList.Add(node); //adiciona-se à lista de visitados
            //e expande-se 
            List<GraphEdge> edges = graph.EdgesAdjacencyListVector[node.Index];
            for (int i=0; i < edges.Count; i++)
            {
                
                GraphNode childNode = graph.Nodes[edges[i].To];
                Debug.Log("Expandindo para nó filho " + childNode.Index);
                if (visitedList.Contains(childNode)) //caso nó filho já tenha sido visitado
                {
                    Debug.Log("ESTE NÓ JA FOI VISITADO, IGNORA-SE");
                    continue;
                }
                //calcula-se A*score dos nós filhos (g + h)-> valor das arestas + h do nó em causa
                double gCost = node.GCost + edges[i].Cost;
                
                if (childNode.HCost == int.MaxValue) //calcula hCost se ainda não o foi
                {
                    childNode.HCost = heuristic(childNode, possibleGoals);
                    Debug.Log("H DE NÓ " + childNode.Index + "= " + childNode.HCost);
                }
                double fCost = childNode.HCost + gCost;

                //Previne casos quando nó filho ja esta na expandable list, com Fscore maior
                if (expandableList.Contains(childNode) )
                {

                    if (childNode.FCost > fCost)
                    {
                        Debug.Log("NÓ FILHO NA OPENLIST, E ENCONTRADO F MENOR");
                        childNode.FCost = fCost;
                        childNode.GCost = gCost;
                        childNode.PreviousPathNode = node;
                        Debug.Log("NOVO F: " + childNode.FCost);
                    }                   
                }
                else 
                {
                    Debug.Log("NÓ " + childNode.Index+" ADICIONADO À OPENLIST, POSSIVEL DE SER EXPANDIDO");
                    childNode.FCost = fCost;
                    childNode.GCost = gCost;
                    childNode.PreviousPathNode = node;
                    expandableList.Add(childNode); //adiciona-se nós filhos à lista de nós expandíveis
                }                   
            }
        }
        return null; //se já se visitou todos os nós possíveis e nenhum deles é objetivo
    }

    //Passível de optimização, efetuando procura binária
    private static GraphNode GetLowestFCostNode(List<GraphNode> list)
    {
        GraphNode best = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].FCost < best.FCost)
            {
                best = list[i];
            }
        }
        return best;
    }

    private static List<GraphNode> GetPath(GraphNode goalNode)
    {
        List<GraphNode> solution = new List<GraphNode>();
        solution.Add(goalNode);
        GraphNode currentNode = goalNode;
        while (currentNode.PreviousPathNode != null)
        {
            solution.Add(currentNode.PreviousPathNode);
            currentNode = currentNode.PreviousPathNode;
        }
        solution.Reverse();
        return solution;
    }

    public static void DebugAStarList(List<GraphNode> list)
    {
        Debug.Log("IN THIS LIST: (with " + list.Count + " nodes)");
        string result = null;
        foreach(GraphNode node in list)
        {
            result += "||NODE " + node.Index + " (F = " + node.FCost
                + ", G = " + node.GCost + ", H = " + node.HCost;
        }
        Debug.Log(result);
    }
    public static int CalculateManhattanDistance(int[] start, int[] end)
    {   
        return Mathf.Abs(end[0] - start[0]) + Mathf.Abs(end[1] - start[1]);
    }

    public static List<int[]> GetNeighbouringTiles(Grid grid, int x, int y)
    {
        List<int[]> neighbours = new List<int[]>();
        if (y + 1 < grid.Array.GetLength(1))
        {
            neighbours.Add(new int[2] {x,y + 1 });
        }
        if (x + 1 < grid.Array.GetLength(0))
        {
            neighbours.Add(new int[2] {x + 1 ,y});
        }
        if (y - 1 >= 0)
        {
            neighbours.Add(new int[2] {x,y - 1 });
        }
        if (x - 1 >= 0)
        {
            neighbours.Add(new int[2] {x-1, y});
        }
        return neighbours;
    }

    #endregion

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
