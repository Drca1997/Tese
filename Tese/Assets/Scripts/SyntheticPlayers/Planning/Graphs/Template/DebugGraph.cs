using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGraph : MonoBehaviour 
{
    private void Start()
    {
        Graph graph = CreateGraph(8);
        List<GraphNode> goals = new List<GraphNode>();
        goals.Add(graph.Nodes[6]);
        goals.Add(graph.Nodes[7]);
        List<GraphNode> path = AStarClass.AStarPathFinding(graph, graph.Nodes[0], goals, ExampleHeuristic2);
        Debug.Log("CAMINHO ENCONTRADO");
        AStarClass.DebugAStarList(path);
    }

    public Graph CreateGraph(int n)
    {
        Graph graph = new Graph(n);
        for (int i=0; i < n; i++) //creation of nodes
        {
            graph.Nodes.Add(new GraphNode(i));
        }
        //creation of edges
        graph.EdgesAdjacencyListVector[0].Add(new GraphEdge(0, 1, 2));
        graph.EdgesAdjacencyListVector[0].Add(new GraphEdge(0, 3, 3));
        graph.EdgesAdjacencyListVector[1].Add(new GraphEdge(1, 2, 1));
        graph.EdgesAdjacencyListVector[1].Add(new GraphEdge(1, 5, 8));
        graph.EdgesAdjacencyListVector[2].Add(new GraphEdge(2, 0, 2));
        graph.EdgesAdjacencyListVector[2].Add(new GraphEdge(2, 3, 1));
        graph.EdgesAdjacencyListVector[2].Add(new GraphEdge(2, 4, 1));
        graph.EdgesAdjacencyListVector[2].Add(new GraphEdge(2, 6, 4));
        graph.EdgesAdjacencyListVector[3].Add(new GraphEdge(3, 4, 1));
        graph.EdgesAdjacencyListVector[3].Add(new GraphEdge(3, 7, 5));
        graph.EdgesAdjacencyListVector[4].Add(new GraphEdge(4, 6, 5));
        graph.EdgesAdjacencyListVector[4].Add(new GraphEdge(4, 7, 2));
        graph.EdgesAdjacencyListVector[5].Add(new GraphEdge(5, 6, 9));
        graph.EdgesAdjacencyListVector[5].Add(new GraphEdge(5, 7, 7));
        
        /*
        graph.EdgesAdjacencyListVector[0].Add(new GraphEdge(0, 1, 5));
        graph.EdgesAdjacencyListVector[0].Add(new GraphEdge(0, 2, 9));
        graph.EdgesAdjacencyListVector[0].Add(new GraphEdge(0, 4, 6));
        graph.EdgesAdjacencyListVector[1].Add(new GraphEdge(1, 2, 3));
        graph.EdgesAdjacencyListVector[1].Add(new GraphEdge(1, 7, 9));
        graph.EdgesAdjacencyListVector[2].Add(new GraphEdge(2, 3, 1));
        graph.EdgesAdjacencyListVector[2].Add(new GraphEdge(2, 1, 2));
        graph.EdgesAdjacencyListVector[3].Add(new GraphEdge(3, 0, 6));
        graph.EdgesAdjacencyListVector[3].Add(new GraphEdge(3, 6, 7));
        graph.EdgesAdjacencyListVector[3].Add(new GraphEdge(3, 8, 5));
        graph.EdgesAdjacencyListVector[4].Add(new GraphEdge(4, 0, 1));
        graph.EdgesAdjacencyListVector[4].Add(new GraphEdge(4, 5, 2));
        graph.EdgesAdjacencyListVector[4].Add(new GraphEdge(4, 3, 2));
        graph.EdgesAdjacencyListVector[5].Add(new GraphEdge(5, 9, 7));
        graph.EdgesAdjacencyListVector[6].Add(new GraphEdge(6, 4, 2));
        graph.EdgesAdjacencyListVector[6].Add(new GraphEdge(6, 9,8 ));
        */
        return graph;
    }

    public double ExampleHeuristic(GraphNode node, List<GraphNode> goals)
    {
        switch (node.Index)
        {
            case 0:
            case 5: 
                return 5;
            case 1:
                return 7;
            case 2:
                return 3;
            case 3:
                return 4;
            case 4:
            case 6:
                return 6;
            case 7:
            case 8:
            case 9:
                return 0;
            default:
                return -1;
            
        }
    }
    public double ExampleHeuristic2(GraphNode node, List<GraphNode> goals)
    {
        switch (node.Index)
        {
            case 0:
                return 5;
            case 1:
                return 2;
            case 2:
            case 4:
                return 1;
            case 3:
                return 3;
            case 5:
                return 6;
            case 6:
            case 7:
                return 0;
            default:
                return -1;

        }
    }
}
