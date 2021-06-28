using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{

    private List<GraphNode> nodes;
    //o atributo index de cada nó corresponde ao índice do vetor. 
    //Cada item do vetor é uma Lista de arestas que conectam esse nó a outros
    private List<GraphEdge>[] edgesAdjacencyListVector; 

    public Graph(int n)
    {

        nodes = new List<GraphNode>();
        edgesAdjacencyListVector = new List<GraphEdge>[n];
        for (int i=0; i < n; i++)
        {
            edgesAdjacencyListVector[i] = new List<GraphEdge>();
        }
    }

    public List<GraphNode> Nodes { get => nodes; set => nodes = value; }
    public List<GraphEdge>[] EdgesAdjacencyListVector { get => edgesAdjacencyListVector; set => edgesAdjacencyListVector = value; }
}
