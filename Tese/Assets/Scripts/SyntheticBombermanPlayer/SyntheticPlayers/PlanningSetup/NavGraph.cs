using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NavGraph
{
    public enum Tile
    {
        Player, PlayerEnemy, AIEnemy, Walkable, Explodable, Unsurpassable, Bomb, Fire,
        PlayerNBomb, PlayerEnemyNBomb, AIEnemyNBomb,
        FireNExplodable, FireNPlayer, FireNPlayerEnemy, FireNAIEnemy, FireNBomb,
        FireNBombNPlayer, FireNBombNPlayerEnemy, FireNBombNAIEnemy
    }

    public static List<GraphNode> GetPath(int[,] grid, int agentX, int agentY, Goal goal)
    {
        Graph graph = CreateGraph(grid, goal);
        //DebugGraph(graph);
        return AStar.AStarPathFinding(graph, GetStart(graph, grid.GetLength(0), agentX, agentY), GetGoals(grid, agentX, agentY, graph, goal), AStar.ManhattanDistanceHeuristic);

    }

    private static Graph CreateGraph(int[,] grid, Goal goal)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        Graph graph = new Graph(width * height);
        for (int i = 0; i < width * height; i++)
        {
            graph.Nodes.Add(new GraphNode(i)); //Cria N�s 
            List<int> neighboursIndexes = SyntheticPlayerUtils.GetNeighbouringTilesIndexes(grid, i);
            for (int k = 0; k < neighboursIndexes.Count; k++) //cria edges
            {
                int[] tile = SyntheticPlayerUtils.GetTileFromIndex(neighboursIndexes[k], width);
                switch (grid[tile[0], tile[1]]) //PERCEBER BEM QUE EDGES � QUE DEVEM SER FEITOS
                {
                    case (int)Tile.Walkable:
                    case (int)Tile.Bomb:
                    case (int)Tile.Player:
                        graph.EdgesAdjacencyListVector[i].Add(new GraphEdge(i, neighboursIndexes[k], 1));
                        break;
                    case (int)Tile.Explodable:
                        //graph.EdgesAdjacencyListVector[i].Add(new GraphEdge(i, neighboursIndexes[k], 5));
                        break;
                    case (int)Tile.AIEnemy:
                        if (goal.GetType() == typeof(AttackEnemyGoal))
                        {
                            graph.EdgesAdjacencyListVector[i].Add(new GraphEdge(i, neighboursIndexes[k], 1));
                        }
                        break;
                    /*case (int)Tile.PlayerNBomb:
                        if (goal.GetType() == typeof(BeSafeGoal))
                        {
                            graph.EdgesAdjacencyListVector[i].Add(new GraphEdge(i, neighboursIndexes[k], 1));
                        }
                        break;*/
                }
            }
        }
        return graph;
    }

    private static GraphNode GetStart(Graph graph, int gridWidth, int agentX, int agentY)
    {

        return graph.Nodes[agentX * gridWidth + agentY];
    }


    public static List<GraphNode> GetGoals(int[,] grid, int agentX, int agentY, Graph graph, Goal goal)
    {
        List<GraphNode> goals = new List<GraphNode>();

        if (goal.GetType() == typeof(AttackEnemyGoal))
        {
            //if (goal.RefTile == null)
            //{
                foreach (int[] tile in SyntheticPlayerUtils.GridIterator(grid))
                {
                    if (grid[tile[0], tile[1]] == (int)Tile.AIEnemy && (tile[0] != agentX || tile[1] != agentY))
                    {
                        goals.Add(graph.Nodes[tile[0] * grid.GetLength(0) + tile[1]]);
                    }
                }

            /*}
            else
            {
                goals.Add(graph.Nodes[goal.RefTile[0] * grid.GetLength(0) + goal.RefTile[1]]);

            }*/

        }
        else if (goal.GetType() == typeof(BeSafeGoal))
        {
            //todas as tiles que nao sao dangerTiles
            //List<int[]> safeTiles = Utils.dangerTiles(Utils.dangerMap(grid), true);
            foreach (int[] tile in goal.TargetTiles)
            {
                goals.Add(graph.Nodes[tile[0] * grid.GetLength(0) + tile[1]]);
            }
        }
        return goals;
    }

    private static void DebugGraph(Graph graph)
    {
        foreach (GraphNode node in graph.Nodes)
        {
            Debug.Log("N� " + node.Index);
            foreach (GraphEdge edge in graph.EdgesAdjacencyListVector[node.Index])
            {
                Debug.Log("From " + edge.From + " To " + edge.To + " = " + edge.Cost);
            }
        }
    }
}