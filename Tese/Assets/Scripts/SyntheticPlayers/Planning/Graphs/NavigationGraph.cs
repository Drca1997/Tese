using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NavigationGraph
{

    public static List<GraphNode> GetPath(int[,] grid, int agentX, int agentY, GoalTemplate goal)
    {
        Graph graph = CreateGraph(grid);
        //DebugGraph(graph);
        return AStarClass.AStarPathFinding(graph, GetStart(graph, grid.GetLength(0), agentX, agentY), GetGoals(grid, agentX, agentY, graph, goal), AStarClass.ManhattanDistanceHeuristic);
        
    }

    private static Graph CreateGraph(int[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        Graph graph = new Graph(width * height);
        for (int i = 0; i < width * height; i++)
        {
            graph.Nodes.Add(new GraphNode(i)); //Cria Nós 
            List<int> neighboursIndexes = Utils.GetNeighbouringTilesIndexes(grid, i);
            for (int k = 0; k < neighboursIndexes.Count; k++) //cria edges
            {
                int[] tile = Utils.GetTileFromIndex(neighboursIndexes[k], width);
                if (grid[tile[0], tile[1]] == 1 || grid[tile[0], tile[1]] == 4 || grid[tile[0], tile[1]] == 0)
                {
                    graph.EdgesAdjacencyListVector[i].Add(new GraphEdge(i, neighboursIndexes[k], 1));
                }
                else if (grid[tile[0], tile[1]] == 2)
                {
                    graph.EdgesAdjacencyListVector[i].Add(new GraphEdge(i, neighboursIndexes[k], 5));
                }
            }
        }

        

        return graph;
    }

    private static GraphNode GetStart(Graph graph, int gridWidth, int agentX, int agentY)
    {
        
        return graph.Nodes[agentX * gridWidth + agentY];
    }


    public static List<GraphNode> GetGoals(int[,] grid,  int agentX, int agentY, Graph graph, GoalTemplate goal)
    {
        List<GraphNode> goals = new List<GraphNode>();
        
        if (goal.GetType() == typeof(GoalAttackEnemy))
        {
            if (goal.RefTile == null)
            {
                foreach(int[] tile in Utils.GridIterator(grid)) {
                    if (grid[tile[0], tile[1]] == 0 && (tile[0] != agentX || tile[1] != agentY))
                    {
                        goals.Add(graph.Nodes[tile[0] * grid.GetLength(0) + tile[1]]);
                    }
                }
                
            }
            else
            {
                goals.Add(graph.Nodes[goal.RefTile[0] * grid.GetLength(0) + goal.RefTile[1]]);
                
            }
            
        }
        else if (goal.GetType() == typeof(GoalBeSafe))
        {
            //todas as tiles que nao sao dangerTiles
            //List<int[]> safeTiles = Utils.dangerTiles(Utils.dangerMap(grid), true);
            foreach(int [] tile in goal.TargetTiles)
            {
                goals.Add(graph.Nodes[tile[0] * grid.GetLength(0) + tile[1]]);
            }
        }
        return goals;
    }

    private static void DebugGraph(Graph graph)
    {
        foreach(GraphNode node in graph.Nodes)
        {
            Debug.Log("Nó " + node.Index);
            foreach(GraphEdge edge in graph.EdgesAdjacencyListVector[node.Index])
            {
                Debug.Log("From " + edge.From + " To " +  edge.To + " = " +  edge.Cost);
            }
        }
    }
}
